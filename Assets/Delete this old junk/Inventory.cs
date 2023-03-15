using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventory", menuName = "New Inventory")]
public class Inventory : ScriptableObject
{
    [Serializable]
    public class InventoryItemAndAmt {
        public int InventoryItem;
        public int Amount;
    }

    public List<InventoryItemAndAmt> InventoryItems;
}
