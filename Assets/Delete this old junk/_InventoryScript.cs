using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class _InventoryScript : MonoBehaviour
{
    public static event Action<List<_InventoryItem>> OnInventoryChange;
    
    // public List<InventoryItem> inventory = new List<InventoryItem>();
    private readonly Dictionary<ItemSO, _InventoryItem> itemDictionary = new Dictionary<ItemSO, _InventoryItem>();

    private void OnEnable() {
        CollectibleItem.OnItemCollected += Add;
    }

    private void OnDisable() {
        CollectibleItem.OnItemCollected -= Add;
    }

    //If we care about inventory capacity, we'll have to add logic here to make sure you're not holding too much.
    private void Add(ItemSO itemData) {
        // the out param is defining an empty var that will be populated so you can have a
        // boolean return value and an object reference return value at the same time sort of
        if (itemDictionary.TryGetValue(itemData, out var item)) {
            // item.AddToStack();
            OnInventoryChange?.Invoke(itemDictionary.Values.ToList());
        } else {
            // var newItem = new InventoryItem(itemData);
            // inventory.Add(newItem);
            // itemDictionary.Add(itemData, newItem);
            OnInventoryChange?.Invoke(itemDictionary.Values.ToList());
        }
    }

    public void Remove(ItemSO itemData) {
        if (itemDictionary.TryGetValue(itemData, out var item)) {
            // item.RemoveFromStack();
            // if (item.stackSize == 0) {
                itemDictionary.Remove(itemData);
            // }
            OnInventoryChange?.Invoke(itemDictionary.Values.ToList());
        }
    }
}