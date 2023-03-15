using System;
using UnityEngine;

public enum VenomState {
    Idle,
    Walk,
    Run,
    Jump,
    Reach
}

public class VenomAnimatorController : MonoBehaviour {
    public static event Action<VenomState, bool> AnimationFinishedEvent;

    [SerializeField] private GameObject gumObject;

    private Animator animator;
    private VenomState currentState;

    private void Awake() {
        PlayerController.VenomStateEvent += ChangeAnimationState;
    }

    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.name == "Gum") {
            gumObject.SetActive(true);
            col.gameObject.SetActive(false);
        }
    }

    private void ChangeAnimationState(VenomState newState) {
        if (currentState == newState) {
            return;
        }
        animator.Play(newState.ToString());
        currentState = newState;
    }

    private void AnimationFinished() {
        if (gumObject.activeSelf) {
            gumObject.SetActive(false);
            AnimationFinishedEvent?.Invoke(currentState, true);
        } else {
            AnimationFinishedEvent?.Invoke(currentState, false);
        }
    }
}