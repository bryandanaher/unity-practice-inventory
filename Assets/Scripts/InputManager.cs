using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInputActions inputActions;
    public static event Action<InputActionMap> OnActionMapChange;

    private void Awake() {
        InventoryArbiter.OnInventoryActive += HandleInventoryActive;
        inputActions = new PlayerInputActions();
        // DisableActionMaps();
        SetActionMap(inputActions.Player);
    }

    private void OnDisable() {
        InventoryArbiter.OnInventoryActive -= HandleInventoryActive;
        inputActions.Disable();
    }

    // private void DisableActionMaps() {
    //     foreach(var map in inputActions.asset.actionMaps) {
    //         map.Disable();
    //     }
    // }
        
    private void SetActionMap(InputActionMap actionMap) {
        // if (actionMap.enabled) return;
        // DisableActionMaps();
        
        inputActions.Disable();
        actionMap.Enable();
        // Debug.Log("Player enabled: " + inputActions.Player.enabled + "   UI enabled: " + inputActions.UI.enabled);
        OnActionMapChange?.Invoke(actionMap); //Is it important to send this event before actionMap is enabled instead of after?
    }

    private void HandleInventoryActive(bool isActive) {
        if (isActive) {
            SetActionMap(inputActions.UI);
        } else {
            SetActionMap(inputActions.Player);
        }
    }
}
