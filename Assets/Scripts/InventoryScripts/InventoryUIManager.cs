using InventoryScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryUIManager : MonoBehaviour
{
    public InventorySO inventorySO;
    public GameItemLookup gameItemLookup;
    private InventoryUIUtils uiUtils;

    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public GameObject splitModalPrefab;
    
    //This is a list of InventorySlot script objects attached to the InventorySlot prefabs
    private InventorySlotScript[] inventorySlots;

    private InputAction closeInventory;

    private void OnEnable() {
        uiUtils = new InventoryUIUtils();
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
        if (!uiUtils.HoldingItem()) return;
        PutDownDraggedItem(slotId);
        if (!updateLogicalInventory) return;
        inventorySO.PlaceItem(slotId);
    }

    private void HandleItemClicked(GameObject clickedItem) {
        if (uiUtils.HoldingItem()) {
            var heldItemStartLocation = uiUtils.GetHeldItemCoordinatesStart();
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
                PickUpAndDragItem(clickedItem, clickedStackSize);
                DestroyGhostItem(clickedSlotId);
            }
            return;
        }
        PickUpAndDragItem(clickedItem);
        inventorySO.PickUpItem(uiUtils.GetHeldItemCoordinatesEnd());
    }

    private void AddHeldItemToStack(InventoryItem clickedInventoryItem) {
        var clickedItemText = clickedInventoryItem.GetComponentInChildren<TextMeshProUGUI>();
        var heldItemText = uiUtils.GetHeldItem().GetComponentInChildren<TextMeshProUGUI>();
        var numberOfItemsToAdd = inventorySO.AddHeldItemToStack(clickedInventoryItem.GetMoveCoordinates().end);

        clickedItemText.text = (int.Parse(clickedItemText.text) + numberOfItemsToAdd).ToString();
        heldItemText.text = (int.Parse(heldItemText.text) - numberOfItemsToAdd).ToString();
        DestroyGhostItem(uiUtils.GetHeldItemCoordinatesStart());
        if (inventorySO.GetHeldItem() == null) {
            uiUtils.DestroyHeldItemPrefab(); 
        }
    }

    private void OpenSplitMenu(GameObject clickedItem) {
        var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
        var clickedItemIndex = clickedInventoryItem.GetMoveCoordinates().end;

        if (inventorySO.GetItemData(clickedItemIndex).stackSize <= 1 || uiUtils.HoldingItem()) {
            HandleItemClicked(clickedItem);
            return;
        }

        var splitModal = CreateSplitModal();
        var clickedItemStackSize = inventorySO.GetItemData(clickedItemIndex).stackSize;
        var splitMenu = splitModal.transform.Find("SplitStackMenu");
        var itemIcon = splitMenu.transform.Find("ItemIcon").GetComponent<Image>();
        var slider = splitMenu.transform.Find("Slider").GetComponent<Slider>();
        var percentageText = splitMenu.transform.Find("PercentageText").GetComponent<TMP_Text>();

        splitModal.GetComponent<SplitStackMenuController>().clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
        itemIcon.sprite = clickedItem.GetComponent<InventoryItem>().image.sprite;
        slider.maxValue = clickedItemStackSize;
        slider.value = clickedItemStackSize - (clickedItemStackSize / 2);
        percentageText.text = slider.value + "/" + clickedItemStackSize;
    }

    private void SplitStack(GameObject splitModal) {
        var splitMenu = splitModal.transform.Find("SplitStackMenu");
        var slider = splitMenu.transform.Find("Slider").GetComponent<Slider>();
        var clickedInventoryItem = splitModal.GetComponent<SplitStackMenuController>().clickedInventoryItem;
        var clickedItemIndex = clickedInventoryItem.GetMoveCoordinates().end;

        if ((int)slider.value == (int)slider.maxValue) {
            HandleItemClicked(clickedInventoryItem.gameObject);
            return;
        }

        uiUtils.SetHeldItemPrefab(CloneItem(clickedInventoryItem, (int)slider.value));
        uiUtils.GetHeldInventoryItem().OnBeginDrag();
        var adjustedStackSize = inventorySO.GetItemData(clickedItemIndex).stackSize - (int)slider.value;
        inventorySO.GetItemData(clickedItemIndex).stackSize = adjustedStackSize;
        // clickedInventoryItem.GetComponentInChildren<TextMeshProUGUI>().text = adjustedStackSize.ToString();
        SetGhostItem(GetInventoryItemBySlotIndex(clickedItemIndex).gameObject);
    }

    public void PutAwayCarriedItems() {
        if (!uiUtils.HoldingItem()) return;
        var startIndex = uiUtils.GetHeldItemCoordinatesStart();
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

    private void SetGhostItem(GameObject itemObject) {
        itemObject.GetComponent<Image>().color = new Color32(255, 255, 225, 100);
        itemObject.GetComponent<InventoryItem>().ghostItem = true;
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
        if (!uiUtils.HoldingItem()) return;
        var inventoryItem = uiUtils.GetHeldInventoryItem();
        PutInventoryItemInSlot(inventoryItem, clickedSlotId, false);
        uiUtils.DestroyHeldItemPrefab();
    }
    
    private void PickUpAndDragItem(GameObject clickedItem, int stackSize = 0) {
        var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
        var clickedItemIndex = clickedInventoryItem.GetMoveCoordinates().end;
        if (stackSize == 0) {
            stackSize = inventorySO.GetItemData(clickedItemIndex).stackSize;
        }
        uiUtils.SetHeldItemPrefab(CloneItem(clickedInventoryItem, stackSize));
        uiUtils.GetHeldInventoryItem().OnBeginDrag();
        SetGhostItem(clickedItem);
    }

    private void PutInventoryItemInSlot(InventoryItem inventoryItem, int destinationIndex,
        bool setCoordinates = true) {
        DestroyGhostItem(destinationIndex);
        DestroyGhostItem(inventoryItem.GetMoveCoordinates().start);
        if (setCoordinates) {
            var newCoordinates = (inventoryItem.GetMoveCoordinates().end,
                inventoryItem.GetMoveCoordinates().end);
            inventoryItem.SetMoveCoordinates(newCoordinates);
        }
        inventoryItem.parentAfterDrag = inventorySlots[destinationIndex].transform;
        inventoryItem.OnEndDrag();
    }

    private GameObject CreateSplitModal() {
        var splitModal = Instantiate(splitModalPrefab, transform.parent.parent);
        var modalRt = splitModal.GetComponent<RectTransform>();
        modalRt.anchorMin = new Vector2(0, 0);
        modalRt.anchorMax = new Vector2(1, 1);
        modalRt.offsetMin = Vector2.zero;
        modalRt.offsetMax = Vector2.zero;
        return splitModal;
    }

    private GameObject CloneItem(InventoryItem inventoryItem, int stackSize) {
        var inventoryItemData = inventorySO.GetItemData(inventoryItem.GetMoveCoordinates().end);
        var parentTransform = inventorySlots[inventoryItem.GetMoveCoordinates().end].gameObject.transform;
        var itemClassName = inventoryItemData.ItemObject.name;
        var itemData = new ItemData(gameItemLookup.FindItemByObjectName(itemClassName), stackSize);
        inventorySO.SetHeldItem(itemData);
        return uiUtils.CloneItem(itemPrefab, parentTransform, itemData, stackSize, inventoryItem.GetMoveCoordinates());
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
        ResetInventory(inventory.Length);
        for (var i = 0; i < inventory.Length; i++) {
            CreateInventorySlot(i);
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

    private void CreateInventorySlot(int index) {
        //The second parameter sets the parent, instead of instantiating by default in a
        //brand new transform hierarchy and then setting it to the proper parent afterwards
        var newSlot = Instantiate(slotPrefab, transform);

        //Get the InventorySlot script attached to the newly instantiated prefab
        var inventorySlotScript = newSlot.GetComponent<InventorySlotScript>();
        inventorySlotScript.id = index;

        inventorySlots[index] = inventorySlotScript;
    }

    public bool WillThisMethodReturnTrue(bool prediction) {
        return prediction;
    }
}