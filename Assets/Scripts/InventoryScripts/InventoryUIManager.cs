using InventoryScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryUIManager : MonoBehaviour 
{ 
    public InventorySO inventorySO;
    public GameItemLookup gameItemLookup;
    // private InventoryUIUtils uiUtils;
    private HeldItemManager heldItemManager;

    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public GameObject splitModalPrefab;

    //This is a list of InventorySlot script objects attached to the InventorySlot prefabs
    private InventorySlotScript[] inventorySlots;

    private InputAction closeInventory;

    private void OnEnable() {
        // uiUtils ??= new InventoryUIUtils();
        heldItemManager ??= new HeldItemManager();
        InventorySlotScript.OnSlotClicked += HandleSlotClicked;
        InventoryItem.OnItemClicked += HandleItemClicked;
        InventoryItem.OnItemShiftClick += OpenSplitMenu;
        SplitStackMenuController.OnSplitStack += SplitStack;
        InventoryArbiter.OnInventoryChange += UpdateInventory;
    }

    private void OnDisable() {
        InventorySlotScript.OnSlotClicked -= HandleSlotClicked;
        InventoryItem.OnItemClicked -= HandleItemClicked;
        InventoryItem.OnItemShiftClick -= OpenSplitMenu;
        SplitStackMenuController.OnSplitStack -= SplitStack;
        InventoryArbiter.OnInventoryChange -= UpdateInventory;
    }

    private void HandleSlotClicked(int slotId, bool updateLogicalInventory = true) {
        if (!heldItemManager.HoldingItem()) return;
        PutDownDraggedItem(slotId);
        if (!updateLogicalInventory) return;
        inventorySO.PlaceItem(slotId);
    }

    private void HandleItemClicked(GameObject clickedItem) {
        if (heldItemManager.HoldingItem()) {
            var heldItemStartLocation = heldItemManager.GetHeldItemCoordinatesStart();
            var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();

            if (CanAddToItemStack(clickedInventoryItem)) {
                AddHeldItemToStack(clickedInventoryItem);
            } else if (IsSlotEmpty(heldItemStartLocation)) {
                ItemsSwitchPlaces(clickedItem, heldItemStartLocation);
            } else {
                var clickedSlotId = clickedInventoryItem.GetMoveCoordinates().end;
                var clickedStackSize = inventorySO.GetItemData(clickedSlotId).stackSize;
                PutDownDraggedItem(clickedSlotId);
                inventorySO.PlaceItem(clickedSlotId);
                PickUpAndDragItem(clickedItem, clickedStackSize, true);
                DestroyGhostItem(clickedSlotId);
            }
            return;
        }
        PickUpAndDragItem(clickedItem);
        inventorySO.PickUpItem(heldItemManager.GetHeldItemCoordinatesEnd());
    }

    private void AddHeldItemToStack(InventoryItem clickedInventoryItem) {
        var clickedItemText = clickedInventoryItem.GetComponentInChildren<TextMeshProUGUI>();
        var heldItemText = heldItemManager.GetHeldItem().GetComponentInChildren<TextMeshProUGUI>();
        var numberOfItemsToAdd = inventorySO.AddHeldItemToStack(clickedInventoryItem.GetMoveCoordinates().end);

        clickedItemText.text = (int.Parse(clickedItemText.text) + numberOfItemsToAdd).ToString();
        heldItemText.text = (int.Parse(heldItemText.text) - numberOfItemsToAdd).ToString();
        DestroyGhostItem(heldItemManager.GetHeldItemCoordinatesStart());
        if (inventorySO.GetHeldItem() == null) {
            heldItemManager.DestroyHeldItemPrefab();
        }
    }

    private void OpenSplitMenu(GameObject clickedItem) {
        var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
        var clickedItemIndex = clickedInventoryItem.GetMoveCoordinates().end;
        var clickedItemStackSize = inventorySO.GetItemData(clickedItemIndex).stackSize;
        if (!InventoryUIUtils.OpenSplitMenu(clickedItem, splitModalPrefab, transform.parent.parent, clickedItemStackSize, heldItemManager)) {
            HandleItemClicked(clickedItem);
        }
    }

    private void SplitStack(GameObject splitModal) {
        var clickedInventoryItem = splitModal.GetComponent<SplitStackMenuController>().clickedInventoryItem;
        (int selectedValue, int maxValue) sliderValue = InventoryUIUtils.SplitModalSelectedValue(splitModal);

        if (sliderValue.selectedValue == sliderValue.maxValue) {
            HandleItemClicked(clickedInventoryItem.gameObject);
            return;
        }

        var clickedItemIndex = clickedInventoryItem.GetMoveCoordinates().end;
        heldItemManager.SetHeldItemPrefab(CloneItem(clickedInventoryItem, sliderValue.selectedValue));
        heldItemManager.GetHeldInventoryItem().OnBeginDrag();
        var adjustedStackSize = inventorySO.GetItemData(clickedItemIndex).stackSize - sliderValue.selectedValue;
        inventorySO.GetItemData(clickedItemIndex).stackSize = adjustedStackSize;
        InventoryUIUtils.SetGhostItem(GetInventoryItemBySlotIndex(clickedItemIndex).gameObject);
    }

    public void PutAwayCarriedItems() {
        if (!heldItemManager.HoldingItem()) return;
        var startIndex = heldItemManager.GetHeldItemCoordinatesStart();
        if (inventorySO.GetItemData(startIndex) != null) {
            //TODO: replace this with a de-ghost method, depending on how I implement
            AddHeldItemToStack(GetInventoryItemBySlotIndex(startIndex));
        } else {
            HandleSlotClicked(startIndex);
        }
    }

    /*
    /---------------------UTILITY/SECONDARY ACTIONS---------------------
    */
    private InventoryItem GetInventoryItemBySlotIndex(int index) {
        return inventorySlots[index].gameObject.GetComponentInChildren<InventoryItem>();
    }

    private void ItemsSwitchPlaces(GameObject clickedItem, int draggedItemOrigin) {
        var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
        var clickedSlotId = clickedInventoryItem.GetMoveCoordinates().end;
        PutInventoryItemInSlot(clickedInventoryItem, draggedItemOrigin, false);
        HandleSlotClicked(clickedSlotId, false);

        inventorySO.SwitchItems(clickedSlotId, draggedItemOrigin);
        inventorySO.PlaceItem(clickedSlotId);
    }

    private bool CanAddToItemStack(InventoryItem clickedInventoryItem) {
        var clickedItemIndex = clickedInventoryItem.GetMoveCoordinates().end;
        return inventorySO.ItemExists(clickedItemIndex) &&
               inventorySO.GetItemData(clickedItemIndex).itemName == inventorySO.GetHeldItem().itemName &&
               inventorySO.GetItemData(clickedItemIndex).stackSize < inventorySO.stackSizeMax;
    }

    private bool IsSlotEmpty(int index) {
        var childCount = inventorySlots[index].gameObject.transform.childCount;
        return childCount <= 0 || (inventorySlots[index].HasGhostItems() && !inventorySO.ItemExists(index));
    }

    // private void UnSetGhostItem(GameObject itemObject) {
    //     itemObject.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
    //     itemObject.GetComponent<InventoryItem>().ghostItem = false;
    // }

    private void DestroyGhostItem(int index) {
        if (inventorySO.ItemExists(index) && inventorySlots[index].HasGhostItems()) {
            var incompleteItemData = inventorySO.GetItemData(index);
            inventorySlots[index].DrawSlot(new ItemData(gameItemLookup.FindItemByObjectName(incompleteItemData.ItemObject.name),
                incompleteItemData.stackSize));
        } else {
            var ghostSlot = inventorySlots[index];
            ghostSlot.ClearGhostItems();
        }
    }

    private void PutDownDraggedItem(int clickedSlotId) {
        if (!heldItemManager.HoldingItem()) return;
        var inventoryItem = heldItemManager.GetHeldInventoryItem();
        PutInventoryItemInSlot(inventoryItem, clickedSlotId, false);
        heldItemManager.ResetHeldItemPrefab();
    }

    private void PickUpAndDragItem(GameObject clickedItem, int stackSize = 0, bool held = false) {
        var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
        var clickedItemIndex = clickedInventoryItem.GetMoveCoordinates().end;
        if (stackSize == 0) {
            stackSize = inventorySO.GetItemData(clickedItemIndex).stackSize;
        }
        heldItemManager.SetHeldItemPrefab(CloneItem(clickedInventoryItem, stackSize, held));
        heldItemManager.GetHeldInventoryItem().OnBeginDrag();
        InventoryUIUtils.SetGhostItem(clickedItem);
    }

    private void PutInventoryItemInSlot(InventoryItem inventoryItem, int destinationIndex, bool setCoordinates = true) {
        DestroyGhostItem(destinationIndex);
        DestroyGhostItem(inventoryItem.GetMoveCoordinates().start);
        var parentAfterDrag = inventorySlots[destinationIndex].transform;
        InventoryUIUtils.PutInventoryItemInSlot(inventoryItem, parentAfterDrag, setCoordinates);
    }


    private GameObject CloneItem(InventoryItem inventoryItem, int stackSize, bool held = false) {
        var inventoryItemData = inventorySO.GetItemData(inventoryItem.GetMoveCoordinates().end);
        if (held) {
            inventoryItemData = inventorySO.GetHeldItem();
        }
        var parentTransform = inventorySlots[inventoryItem.GetMoveCoordinates().end].gameObject.transform;
        var itemClassName = inventoryItemData.ItemObject.name;
        var itemData = new ItemData(gameItemLookup.FindItemByObjectName(itemClassName), stackSize);
        inventorySO.SetHeldItem(itemData);
        return InventoryUIUtils.CloneItem(itemPrefab, parentTransform, itemData, stackSize, inventoryItem.GetMoveCoordinates());
    }

    /*
    /---------------------SETUP/TEARDOWN---------------------
    */
    private void UpdateInventory(ItemData[] inventory) {
        for (var i = 0; i < inventory.Length; i++) {
            if (inventory[i] != null && inventory[i].stackSize > 0) { //TODO: This is apparently always true and I am filled with rage
                inventorySlots[i].DrawSlot(inventory[i]);
            }
        }
    }

    public void DrawInventory(ItemData[] inventory) {
        heldItemManager ??= new HeldItemManager();

        ResetInventory(inventory.Length);
        for (var i = 0; i < inventory.Length; i++) {
            inventorySlots[i] = InventoryUIUtils.CreateInventorySlot(i, slotPrefab, transform);
            //CreateInventorySlot(i);
            if (inventory[i] != null && inventory[i].stackSize > 0) {
                inventorySlots[i].DrawSlot(inventory[i]);
            }
        }
    }

    // Inefficiently clear out the inventory. Bad for a large project apparently.
    private void ResetInventory(int inventoryLength) {
        foreach (Transform childTransform in transform) {
            Destroy(childTransform.gameObject);
        }
        inventorySlots = new InventorySlotScript[inventoryLength];
    }
}