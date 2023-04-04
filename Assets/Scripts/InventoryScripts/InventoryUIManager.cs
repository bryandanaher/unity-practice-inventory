using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InventoryUIManager : MonoBehaviour
{
    public InventorySO inventorySO;
    public GameItemLookup gameItemLookup;

    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public GameObject splitModalPrefab;

    private GameObject heldItemPrefab;

    //This is a list of InventorySlot script objects attached to the InventorySlot prefabs
    private InventorySlotScript[] inventorySlots;

    private InputAction closeInventory;

    private void OnEnable() {
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
        if (heldItemPrefab == null) return;
        PutDownDraggedItem(slotId);
        if (!updateLogicalInventory) return;
        inventorySO.PlaceItem(slotId);
    }

    private void HandleItemClicked(GameObject clickedItem) {
        if (heldItemPrefab != null) {
            var draggedItemCoordinates = heldItemPrefab.GetComponent<InventoryItem>().GetMoveCoordinates();
            var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();

            if (CanAddToItemStack(clickedInventoryItem)) {
                AddHeldItemToStack(clickedInventoryItem);
            } else if (IsSlotEmpty(draggedItemCoordinates.start)) {
                ItemsSwitchPlaces(clickedItem, draggedItemCoordinates.start);
            } else {
                var clickedSlotId = clickedInventoryItem.GetMoveCoordinates().end;
                PutDownDraggedItem(clickedSlotId);
                inventorySO.PlaceItem(clickedSlotId);
                // This is awkward because the logical inventory is showing a different stack size at this point
                // DrawInventory is sort of a nuclear option, but I am tired...
                DrawInventory(inventorySO.ItemArray(), inventorySO.GetHeldItem());
                // PickUpAndDragItem(clickedItem);
            }
            return;
        }
        PickUpAndDragItem(clickedItem);
        var draggedInventoryItem = heldItemPrefab.GetComponent<InventoryItem>();
        inventorySO.PickUpItem(draggedInventoryItem.GetMoveCoordinates().end);
    }

    private void AddHeldItemToStack(InventoryItem clickedInventoryItem) {
        var clickedItemText = clickedInventoryItem.GetComponentInChildren<TextMeshProUGUI>();
        var heldItemText = heldItemPrefab.GetComponentInChildren<TextMeshProUGUI>();
        var numberOfItemsToAdd = inventorySO.AddHeldItemToStack(clickedInventoryItem.GetMoveCoordinates().end);

        clickedItemText.text = (int.Parse(clickedItemText.text) + numberOfItemsToAdd).ToString();
        heldItemText.text = (int.Parse(heldItemText.text) - numberOfItemsToAdd).ToString();
        DestroyGhostItem(heldItemPrefab.GetComponent<InventoryItem>().GetMoveCoordinates().start);
        if (inventorySO.GetHeldItem() == null) {
            Destroy(heldItemPrefab);
        }
    }

    private void OpenSplitMenu(GameObject clickedItem) {
        var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
        var clickedItemIndex = clickedInventoryItem.GetMoveCoordinates().end;

        if (inventorySO.GetItemData(clickedItemIndex).stackSize <= 1 || heldItemPrefab != null) {
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

        heldItemPrefab = CloneItem(clickedInventoryItem, (int)slider.value);
        heldItemPrefab.GetComponent<InventoryItem>().OnBeginDrag();
        var adjustedStackSize = inventorySO.GetItemData(clickedItemIndex).stackSize - (int)slider.value;
        inventorySO.GetItemData(clickedItemIndex).stackSize = adjustedStackSize;
        // clickedInventoryItem.GetComponentInChildren<TextMeshProUGUI>().text = adjustedStackSize.ToString();
        SetGhostItem(GetInventoryItemBySlotIndex(clickedItemIndex).gameObject);
    }

    public void PutAwayCarriedItems() {
        if (heldItemPrefab == null) return;
        var startIndex = heldItemPrefab.GetComponent<InventoryItem>().GetMoveCoordinates().start;
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
        if (inventorySO.ItemExists(index)) {
            var incompleteItemData = inventorySO.GetItemData(index);
            inventorySlots[index].DrawSlot(new ItemData(gameItemLookup.FindItemByObjectName(incompleteItemData.ItemObject.name),
                incompleteItemData.stackSize));
        } else {
            var ghostSlot = inventorySlots[index];
            ghostSlot.ClearGhostItems();
        }
    }

    private void PutDownDraggedItem(int clickedSlotId) {
        if (heldItemPrefab == null) return;
        var inventoryItem = heldItemPrefab.GetComponent<InventoryItem>();
        PutInventoryItemInSlot(inventoryItem, clickedSlotId, false);
        heldItemPrefab = null;
    }
    
    private void PickUpAndDragItem(GameObject clickedItem) {
        var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
        var clickedItemIndex = clickedInventoryItem.GetMoveCoordinates().end;
        var stackSize = inventorySO.GetItemData(clickedItemIndex).stackSize;
        heldItemPrefab = CloneItem(clickedInventoryItem, stackSize);
        heldItemPrefab.GetComponent<InventoryItem>().OnBeginDrag();
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
        var newItem = Instantiate(itemPrefab, parentTransform);
        inventorySO.SetHeldItem(itemData);

        newItem.GetComponent<Image>().sprite = itemData.ItemObject.icon;
        newItem.GetComponentInChildren<TextMeshProUGUI>().text = stackSize.ToString();
        var newInventoryItem = newItem.GetComponent<InventoryItem>();
        newInventoryItem.parentAfterDrag = parentTransform;
        newInventoryItem.SetMoveCoordinates(inventoryItem.GetMoveCoordinates());
        return newItem;
    }

    /*
    /---------------------SETUP/TEARDOWN---------------------
    */
    //TODO: Give this new item move coordinates...somehow.
    private void DrawHeldItem(ItemData heldItemData) {
        var itemClassName = heldItemData.ItemObject.name;
        var itemData = new ItemData(gameItemLookup.FindItemByObjectName(itemClassName), heldItemData.stackSize);
        var newItem = Instantiate(itemPrefab, transform);
        newItem.GetComponent<Image>().sprite = itemData.ItemObject.icon;
        newItem.GetComponentInChildren<TextMeshProUGUI>().text = itemData.stackSize.ToString();
        var newInventoryItem = newItem.GetComponent<InventoryItem>();
        newInventoryItem.parentAfterDrag = transform;
        // newInventoryItem.SetMoveCoordinates(inventoryItem.GetMoveCoordinates());
        
        heldItemPrefab = newItem;
        heldItemPrefab.GetComponent<InventoryItem>().OnBeginDrag();
    }
    
    private void UpdateInventory(ItemData[] inventory) {
        for (var i = 0; i < inventory.Length; i++) {
            if (inventory[i] != null && inventory[i].stackSize > 0) { //TODO: This is apparently always true and I am filled with rage
                inventorySlots[i].DrawSlot(inventory[i]);
            }
        }
    }

    public void DrawInventory(ItemData[] inventory, ItemData heldItem = null) {
        ResetInventory(inventory.Length);
        for (var i = 0; i < inventory.Length; i++) {
            CreateInventorySlot(i);
            if (inventory[i] != null && inventory[i].stackSize > 0) {
                inventorySlots[i].DrawSlot(inventory[i]);
            }
        }
        if (heldItem != null) {
            DrawHeldItem(heldItem);
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
}