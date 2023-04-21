using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPropagator : MonoBehaviour
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