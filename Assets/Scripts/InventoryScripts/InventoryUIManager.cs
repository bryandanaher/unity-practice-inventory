using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public InventorySO inventorySO;

    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public GameObject splitModalPrefab;

    //TODO: name this more consistently with the SO version
    private GameObject itemBeingDragged;

    //This is a list of InventorySlot script objects attached to the InventorySlot prefabs
    private InventorySlotScript[] inventorySlots;

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
        if (itemBeingDragged == null) return;
        var inventoryItem = itemBeingDragged.GetComponent<InventoryItem>();
        inventoryItem.parentAfterDrag = inventorySlots[slotId].transform;
        inventoryItem.OnEndDrag();
        ClearDraggedItem(inventoryItem.GetMoveCoordinates());

        if (!updateLogicalInventory) return;
        inventorySO.PlaceItem(inventoryItem.GetMoveCoordinates().end);
    }

    private void HandleItemClicked(GameObject clickedItem) {
        if (itemBeingDragged != null) {
            var draggedItemCoordinates = itemBeingDragged.GetComponent<InventoryItem>().GetMoveCoordinates();
            var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();

            if (CanAddToItemStack(clickedInventoryItem)) {
                AddHeldItemToStack(clickedInventoryItem);
            } else if (IsSlotEmpty(draggedItemCoordinates.start)) {
                ItemsSwitchPlaces(clickedItem, draggedItemCoordinates.start);
            } else {
                var clickedSlotId = clickedInventoryItem.GetMoveCoordinates().end;
                var inventoryItem = itemBeingDragged.GetComponent<InventoryItem>();
                inventoryItem.parentAfterDrag = inventorySlots[clickedSlotId].transform;
                inventoryItem.OnEndDrag();
                ClearDraggedItem(inventoryItem.GetMoveCoordinates());

                inventorySO.PlaceItem(inventoryItem.GetMoveCoordinates().end);
                // HandleSlotClicked(clickedInventoryItem.GetMoveCoordinates().end);
                itemBeingDragged = clickedItem;
                itemBeingDragged.GetComponent<InventoryItem>().OnBeginDrag();
            }
            return;
        }
        itemBeingDragged = clickedItem;
        inventorySO.PickUpItem(itemBeingDragged.GetComponent<InventoryItem>().GetMoveCoordinates().end);
        clickedItem.GetComponent<InventoryItem>().OnBeginDrag();
    }

    private void ItemsSwitchPlaces(GameObject clickedItem, int draggedItemOrigin) {
        var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
        var clickedSlotId = clickedInventoryItem.GetMoveCoordinates().end;
        TeleportItem(clickedInventoryItem, draggedItemOrigin);
        HandleSlotClicked(clickedSlotId, false);

        inventorySO.SwitchItems(clickedSlotId, draggedItemOrigin);
        inventorySO.PlaceItem(clickedSlotId);
    }

    private bool CanAddToItemStack(InventoryItem clickedInventoryItem) {
        var clickedItemIndex = clickedInventoryItem.GetMoveCoordinates().end;
        return inventorySO.GetItemData(clickedItemIndex).itemName == inventorySO.GetHeldItem().itemName &&
               inventorySO.GetItemData(clickedItemIndex).stackSize < inventorySO.stackSizeMax;
    }

    //If you try to split a stack of 1, it gets all messed up. Except it doesn't.
    //Maybe the shift issue fixed this too?
    private void AddHeldItemToStack(InventoryItem clickedInventoryItem) {
        var clickedItemText = clickedInventoryItem.GetComponentInChildren<TextMeshProUGUI>();
        var heldItemText = itemBeingDragged.GetComponentInChildren<TextMeshProUGUI>();
        var numberOfItemsToAdd = inventorySO.AddHeldItemToStack(clickedInventoryItem.GetMoveCoordinates().end);

        clickedItemText.text = (int.Parse(clickedItemText.text) + numberOfItemsToAdd).ToString();
        heldItemText.text = (int.Parse(heldItemText.text) - numberOfItemsToAdd).ToString();
        if (inventorySO.GetHeldItem() == null) {
            Destroy(itemBeingDragged);
        }
    }

    // private bool ItemStackOverflow(ItemData clickedItemData, ItemData heldItemData) {
    //     return (clickedItemData.stackSize + heldItemData.stackSize) > inventorySO.stackSizeMax;
    // }

    private void OpenSplitMenu(GameObject clickedItem) {
        var clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
        var clickedItemIndex = clickedInventoryItem.GetMoveCoordinates().end;
        var clickedItemStackSize = inventorySO.GetItemData(clickedItemIndex).stackSize;

        if (inventorySO.GetItemData(clickedItemIndex).stackSize > 1 && itemBeingDragged == null) {
            var splitModal = Instantiate(splitModalPrefab, transform.parent.parent);
            var modalRt = splitModal.GetComponent<RectTransform>();
            var splitMenu = splitModal.transform.Find("SplitStackMenu");
            var itemIcon = splitMenu.transform.Find("ItemIcon").GetComponent<Image>();
            var slider = splitMenu.transform.Find("Slider").GetComponent<Slider>();
            var percentageText = splitMenu.transform.Find("PercentageText").GetComponent<TMP_Text>();
            
            modalRt.anchorMin = new Vector2(0, 0);
            modalRt.anchorMax = new Vector2(1, 1);
            modalRt.offsetMin = Vector2.zero;
            modalRt.offsetMax = Vector2.zero;

            splitModal.GetComponent<SplitStackMenuController>().clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
            itemIcon.sprite = clickedItem.GetComponent<InventoryItem>().image.sprite;
            slider.maxValue = clickedItemStackSize;
            slider.value = clickedItemStackSize - (clickedItemStackSize / 2);
            percentageText.text = slider.value + "/" + clickedItemStackSize;
        } else {
            HandleItemClicked(clickedItem);
        }
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
        
        itemBeingDragged = CreateNewDraggedItem(clickedInventoryItem, (int)slider.value);
        itemBeingDragged.GetComponent<InventoryItem>().OnBeginDrag();
        var adjustedStackSize = inventorySO.GetItemData(clickedItemIndex).stackSize - (int)slider.value;
        inventorySO.GetItemData(clickedItemIndex).stackSize = adjustedStackSize;
        clickedInventoryItem.GetComponentInChildren<TextMeshProUGUI>().text = adjustedStackSize.ToString();
    }

    private GameObject CreateNewDraggedItem(InventoryItem inventoryItem, int stackSize) {
        var inventoryItemData = inventorySO.GetItemData(inventoryItem.GetMoveCoordinates().end);
        var parentTransform = inventorySlots[inventoryItem.GetMoveCoordinates().end].gameObject.transform;

        var newItemSO = (ItemSO)ScriptableObject.CreateInstance(typeof(ItemSO));

        newItemSO.displayName = inventoryItemData.ItemObject.displayName;
        newItemSO.icon = inventoryItemData.ItemObject.icon;
        var itemData = new ItemData(newItemSO, stackSize);
        var newItem = Instantiate(itemPrefab, parentTransform);
        inventorySO.SetHeldItem(itemData);

        newItem.GetComponent<Image>().sprite = itemData.ItemObject.icon;
        newItem.GetComponentInChildren<TextMeshProUGUI>().text = stackSize.ToString();
        var newInventoryItem = newItem.GetComponent<InventoryItem>();
        newInventoryItem.parentAfterDrag = parentTransform;
        newInventoryItem.SetMoveCoordinates(inventoryItem.GetMoveCoordinates());
        return newItem;
    }

    private bool IsSlotEmpty(int index) {
        return inventorySlots[index].gameObject.transform.childCount <= 0;
    }

    private void TeleportItem(InventoryItem itemToTeleport, int destinationIndex) {
        var newCoordinates = (itemToTeleport.GetMoveCoordinates().end, itemToTeleport.GetMoveCoordinates().end);
        itemToTeleport.SetMoveCoordinates(newCoordinates);
        itemToTeleport.parentAfterDrag = inventorySlots[destinationIndex].transform;
        itemToTeleport.OnEndDrag();
        //TODO: refactor so I don't have to remember to call this each time
        ClearDraggedItem(itemToTeleport.GetMoveCoordinates());
    }

    //Basically, if the dragged item was put down, we want to set itemBeingDragged to null
    private void ClearDraggedItem((int, int) itemId) {
        if (itemBeingDragged != null &&
            itemId == itemBeingDragged.GetComponent<InventoryItem>().GetMoveCoordinates()) {
            itemBeingDragged = null;
        }
    }

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

    public void PutAwayCarriedItems() {
        if (itemBeingDragged == null) return;
        var startIndex = itemBeingDragged.GetComponent<InventoryItem>().GetMoveCoordinates().start;
        if (inventorySO.GetItemData(startIndex) != null) {
            AddHeldItemToStack(inventorySlots[startIndex].transform.gameObject.GetComponentInChildren<InventoryItem>());
        } else {
            HandleSlotClicked(startIndex);
        }
    }
}