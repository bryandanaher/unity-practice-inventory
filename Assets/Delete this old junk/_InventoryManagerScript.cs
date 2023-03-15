using System.Collections.Generic;
using UnityEngine;

public class _InventoryManagerScript : MonoBehaviour
{
    public GameObject slotPrefab;
    //This is a list of InventorySlot script objects attached to the InventorySlot prefabs
    public List<_InventorySlot> inventorySlots = new List<_InventorySlot>(9);

    private void OnEnable() {
        _InventoryScript.OnInventoryChange += DrawInventory;
    }

    private void Start() {
        DrawInventory(new List<_InventoryItem>());
    }
    
    private void OnDisable() {
        _InventoryScript.OnInventoryChange -= DrawInventory;
    }

    private void DrawInventory(List<_InventoryItem> inventory) {
        ResetInventory();

        Debug.Log("inventorySlots.Capacity: " + inventorySlots.Capacity);

        for (var i = 0; i < inventorySlots.Capacity; i++) {
            CreateInventorySlot();
        }

        for (var i = 0; i < inventory.Count; i++) {
            inventorySlots[i].DrawSlot(inventory[i]);
        }
    }

    // Inefficiently clear out the inventory. Bad for a large project.
    private void ResetInventory() {
        foreach (Transform childTransform in transform) {
            Destroy(childTransform.gameObject); 
        }
        inventorySlots = new List<_InventorySlot>(9);
    }

    private void CreateInventorySlot() {
        //The second parameter sets the parent, instead of instantiating by default in a
        //brand new transform hierarchy and then setting it to the proper parent afterwards
        GameObject newSlot = Instantiate(slotPrefab, transform);
        // newSlot.transform.SetParent(transform, false);

        //Get the InventorySlot script attached to the newly instantiated prefab
        var inventorySlotData = newSlot.GetComponent<_InventorySlot>();
        inventorySlotData.ClearSlot();

        inventorySlots.Add(inventorySlotData);
    }
}
