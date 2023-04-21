// using System.Linq;

using System;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu]
public class InventorySO : ScriptableObject
{
    public int stackSizeMax = 50;
    public int InventorySize { get; set; }

    // ItemData must be marked as serializable or else this won't show up
    [SerializeField] [ItemCanBeNull] private ItemData[] itemArray;

    [SerializeField] private ItemData heldItem;

    public ItemData[] ItemArray() {
        return itemArray;
    }

    public ItemData GetItemData(int index) {
        return itemArray[index];
    }

    public bool ItemExists(int index) {
        return itemArray[index] != null && itemArray[index].stackSize > 0;
    }

    //TODO: revisit the need for this
    public void HandleItemCollected(ItemData itemData) {
        if (!Add(itemData)) {
            return;
        }
    }

    public void PickUpItem(int index) {
        heldItem = itemArray[index];
        itemArray[index] = null;
    }

    public void PlaceItem(int index) {
        if (itemArray[index] == null) {
            itemArray[index] = heldItem;
        } else {
            (itemArray[index], heldItem) = (heldItem, itemArray[index]);
        }
    }

    public void SwitchItems(int index1, int index2) {
        (itemArray[index1], itemArray[index2]) = (itemArray[index2], itemArray[index1]);
    }

    private bool Add(ItemData itemData) {
        return StackSingleItem(itemData) || AddNewItem(itemData);
    }

    private bool StackSingleItem(ItemData itemData) {
        for (var i = 0; i < InventorySize; i++) {
            if (itemArray[i] != null && //Don't know why this null check doesn't work. My working theory is, C# stinks
                itemArray[i].stackSize > 0 && 
                itemArray[i].stackSize < stackSizeMax &&
                itemArray[i].ItemObject.displayName == itemData.ItemObject.displayName) {
                itemArray[i].AddToStack();
                return true;
            }
        }
        return false;
    }

    public int AddHeldItemToStack(int index) {
        if (itemArray[index] != null && 
            itemArray[index].stackSize > 0 && 
            itemArray[index].stackSize < stackSizeMax &&
            itemArray[index].ItemObject.displayName == heldItem.ItemObject.displayName) {
            var remainingCapacity = stackSizeMax - itemArray[index].stackSize;
            var numberToAdd = Math.Min(remainingCapacity, heldItem.stackSize);
            itemArray[index].stackSize += numberToAdd;
            heldItem.stackSize -= numberToAdd;
            if (heldItem.stackSize <= 0) {
                heldItem = null;
            }
            return numberToAdd; //itemArray[index].stackSize;
        }
        return 0;
    }

    private bool AddNewItem(ItemData itemData) {
        for (var i = 0; i < InventorySize; i++) {
            if (itemArray[i] == null || itemArray[i].stackSize <= 0) {
                itemArray[i] = itemData;
                return true;
            }
        }
        return false;
    }
    
    public ItemData GetHeldItem() {
        return heldItem;
    }
    public void SetHeldItem(ItemData item) {
        heldItem = item;
    }

    public void ResetArray() {
        itemArray = new ItemData[InventorySize];
    }
}