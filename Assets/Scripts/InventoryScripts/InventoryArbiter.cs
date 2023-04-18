using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryArbiter : MonoBehaviour
{
    public static event Action<bool> OnInventoryActive;
    public static event Action<ItemData[]> OnInventoryChange;

    public int inventorySize;
    public bool resetOnStartup;

    public GameObject inventoryUIContainer;
    public InventorySO inventorySO;
    private InventoryUIManager uiManager;

    private InputAction inventory;
    private InputAction closeInventory;


    private void OnEnable() {
        CollectibleItem.OnItemCollected += HandleItemCollected;
        InputManager.OnActionMapChange += InitializeInputActions;

        // PlayerController.OnOpenInventory += ToggleUI;
        // InventoryUIManager.OnCloseInventory += ToggleUI;
        inventorySO.InventorySize = inventorySize;

        if (resetOnStartup) {
            inventorySO.ResetArray();
        }
        uiManager = inventoryUIContainer.GetComponentInChildren<InventoryUIManager>();
        uiManager.DrawInventory(new ItemData[inventorySize]);
    }

    private void OnDisable() {
        CollectibleItem.OnItemCollected -= HandleItemCollected;
        // PlayerController.OnOpenInventory -= ToggleUI;
        // InventoryUIManager.OnCloseInventory -= ToggleUI;
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

    private void InventoryOpenClose(InputAction.CallbackContext context) {
        ToggleUI();
    }
    // private void CloseInventory(InputAction.CallbackContext context) {
    //     Debug.Log("Closing inventory...");
    //     OnCloseInventory?.Invoke();
    // }
    //
    // private void OpenInventory(InputAction.CallbackContext context) {
    //     OnOpenInventory?.Invoke();
    // }

    private void InitializeInputActions(InputActionMap actionMap) {
        inventory = InputManager.inputActions.Player.Inventory;
        inventory.started += InventoryOpenClose;
        closeInventory = InputManager.inputActions.UI.Close;
        closeInventory.started += InventoryOpenClose;
    }

    //TODO: make this work maybe
    // private void OnApplicationQuit() {
    //     inventorySO.ResetArray();
    // }
}