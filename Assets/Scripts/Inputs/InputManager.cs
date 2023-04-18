using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInputActions inputActions;
    public static event Action<InputActionMap> OnActionMapChange;

    private void Awake() {
        InputEventPropagator.OnInventoryActive += HandleInventoryActive;
        inputActions = new PlayerInputActions();
        SetActionMap(inputActions.Player);
    }

    private void OnDisable() {
        InputEventPropagator.OnInventoryActive -= HandleInventoryActive;
        inputActions.Disable();
    }

    private static void SetActionMap(InputActionMap actionMap) {
        inputActions.Disable();
        actionMap.Enable();
        OnActionMapChange?.Invoke(actionMap); 
        //Is it important to send this event before actionMap is enabled instead of after?
    }

    private void HandleInventoryActive(bool isActive) {
        if (isActive) {
            SetActionMap(inputActions.UI);
        } else {
            SetActionMap(inputActions.Player);
        }
    }
}
