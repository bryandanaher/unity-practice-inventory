using System;

public class InputEventPropagator
{
    public static event Action<bool> OnInventoryActive;

    public void PropagateInventoryActive(bool isActive) {
        OnInventoryActive?.Invoke(isActive);
    }
}