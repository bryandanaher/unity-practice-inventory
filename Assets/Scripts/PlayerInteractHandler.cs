using System;
using UnityEngine;

public enum PlayerInteractTarget {
    None,
    Carnage,
    Gum,
    Door,
    Doorway,
    DanceHall
}

public class PlayerInteractHandler : MonoBehaviour {
    public static event Action<GameObject> InteractEvent;
    
    public PlayerInteractTarget CurrentInteractTarget => currentInteractTarget;
    private PlayerInteractTarget currentInteractTarget = PlayerInteractTarget.None;

    private bool danceHallInteract = false;

    private void OnTriggerEnter2D(Collider2D col) {
        switch (col.gameObject.name) {
            case "CarnageTalkbox":
                currentInteractTarget = PlayerInteractTarget.Carnage;
                break;
            case "Gum":
                currentInteractTarget = PlayerInteractTarget.Gum;
                break;
            case "Door":
                currentInteractTarget = PlayerInteractTarget.Door;
                break;
            case "Doorway":
                currentInteractTarget = PlayerInteractTarget.Doorway;
                Interact();
                break;
            case "DanceHall":
                currentInteractTarget = PlayerInteractTarget.DanceHall;
                if (!danceHallInteract) {
                    danceHallInteract = true;
                    Interact();
                }
                break;
        }
    }
    
    private void OnTriggerExit2D(Collider2D col) {
        // if (col.gameObject.name == "CarnageTalkbox") {
            currentInteractTarget = PlayerInteractTarget.None;
        // }
    }
    
    public void Interact() {
        if (currentInteractTarget != PlayerInteractTarget.None) {
            InteractEvent?.Invoke(gameObject);
        }
    }
}
