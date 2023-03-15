using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DemogoblinState {
    Steppy = 0,
    Headbang = 1,
    Juggle = 2
}

public class DanceogoblinAnimatorController : MonoBehaviour {
    private Animator animator;
    private DemogoblinState currentState = DemogoblinState.Steppy;

    private void Awake() {
        MinionController.MinionStateEvent += ChangeAnimationState;
    }

    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void ChangeAnimationState() {
        var newState = (int)currentState + 1;
        if (newState == 3) {
            newState = 0;
        }
        currentState = (DemogoblinState)newState;
        animator.Play(currentState.ToString());
    }

    // private void AnimationFinished() { }
}