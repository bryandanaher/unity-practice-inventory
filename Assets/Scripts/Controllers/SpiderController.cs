using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour {
    public static event Action<SpiderState> SpiderStateEvent;

    private SpiderState currentState = SpiderState.Shuffle;

    private void Awake() {
        PlayerInteractHandler.InteractEvent += HandleInteractEvent;
    }

    private void Update() {
        switch (currentState) {
            case SpiderState.Shuffle:
                break;
            case SpiderState.HandsUp:
                break;
            case SpiderState.Pirouette:
                break;
            case SpiderState.Swingin:
                break;
            // case SpiderState.Insecurity:
            //     break;
            case SpiderState.SpiderBooty:
                break;
        }
    }

    private void HandleInteractEvent(GameObject interactingObject) {
        var interactTarget = interactingObject.GetComponent<PlayerInteractHandler>().CurrentInteractTarget;

        if (interactingObject.name == "Player" && interactTarget == PlayerInteractTarget.DanceHall) {
            ChangeState(GetNewState());
        }
    }

    private SpiderState GetNewState() {
        if ((int)currentState >= 5) {
            return (SpiderState)0;
        }
        return (SpiderState)((int)currentState + 1);
    }

    private void ChangeState(SpiderState newState) {
        currentState = newState;
        SpiderStateEvent?.Invoke(newState);
    }
}