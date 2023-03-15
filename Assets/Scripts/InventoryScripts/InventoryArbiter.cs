using System;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryArbiter : MonoBehaviour
{
    public static event Action<bool> OnInventoryActive;
    public static event Action<ItemData[]> OnInventoryChange;

    public int inventorySize;

    public GameObject inventoryUIContainer;
    public InventorySO inventorySO;
    private InventoryUIManager uiManager;
    
    private void OnEnable() {
        CollectibleItem.OnItemCollected += HandleItemCollected;
        inventorySO.InventorySize = inventorySize;
        
        //TODO: make this reset configurable
        inventorySO.ResetArray();
        uiManager = inventoryUIContainer.GetComponentInChildren<InventoryUIManager>();
        uiManager.DrawInventory(new ItemData[inventorySize]);
    }
    
    private void OnDisable() {
        CollectibleItem.OnItemCollected -= HandleItemCollected;
    }

    private void HandleItemCollected(ItemSO itemObject) {
        var newItemData = new ItemData(itemObject);
        inventorySO.HandleItemCollected(newItemData);
        OnInventoryChange?.Invoke(inventorySO.ItemArray());
    }
    
    public void ToggleUI() {
        if (inventoryUIContainer.activeSelf) {
            uiManager.PutAwayCarriedItems();
            inventoryUIContainer.SetActive(false);
            OnInventoryActive?.Invoke(false);
        } else {
            inventoryUIContainer.SetActive(true);
            PretendItemWasCollected();
            OnInventoryActive?.Invoke(true);
        }
    }    
    
    private void PretendItemWasCollected() {
        OnInventoryChange?.Invoke(inventorySO.ItemArray());
    }

    //TODO: make this work maybe
    // private void OnApplicationQuit() {
    //     inventorySO.ResetArray();
    // }
}
