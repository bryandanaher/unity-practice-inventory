using System;

public class EventPropagator
{
    private InputEventPropagator inputEventPropagator = new InputEventPropagator();
    
    private void Awake() {
        InventoryArbiter.OnInventoryActive += PropagateInventoryActive;
    }

    private void OnDisable() {
        InventoryArbiter.OnInventoryActive -= PropagateInventoryActive;
    }

    private void PropagateInventoryActive(bool isActive) {
        inputEventPropagator.PropagateInventoryActive(isActive);
    }
}