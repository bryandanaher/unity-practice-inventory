using System;
using UnityEngine;

public class InputEventPropagator
{
    public static event Action<bool> OnInventoryActive;

    public void PropagateInventoryActive(bool isActive) {
        Debug.Log("PropagateInventoryActive...");
        OnInventoryActive?.Invoke(isActive);
    }
}