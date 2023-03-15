using System;
using UnityEngine;

public class MinionController : MonoBehaviour {
    public static event Action MinionStateEvent;
    
    private void Awake() {
        PlayerInteractHandler.InteractEvent += HandleInteractEvent;
    }

    private void HandleInteractEvent(GameObject interactingObject) {
        var interactTarget = interactingObject.GetComponent<PlayerInteractHandler>().CurrentInteractTarget;
        if (interactingObject.name == "Player" && interactTarget == PlayerInteractTarget.DanceHall) {
            MinionStateEvent?.Invoke();
        }
    }
}
