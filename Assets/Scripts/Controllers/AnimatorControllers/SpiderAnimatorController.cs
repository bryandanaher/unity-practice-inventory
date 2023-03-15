using System;
using UnityEngine;

public enum SpiderState {
    Shuffle = 0,
    HandsUp = 1,
    Pirouette = 2,
    Swingin = 3,
    // Insecurity = 4,
    SpiderBooty = 4
}

public class SpiderAnimatorController : MonoBehaviour {
    public static event Action<SpiderState> AnimationFinishedEvent;

    private Animator animator;
    private SpiderState currentState;
    
    private void Awake() {
        SpiderController.SpiderStateEvent += ChangeAnimationState;
    }

    private void Start() {
        animator = GetComponent<Animator>();
    }
    
    private void ChangeAnimationState(SpiderState newState) {
        if (currentState == newState) {
            return;
        }
        animator.Play(newState.ToString());
        currentState = newState;
    }

    private void AnimationFinished() {
        AnimationFinishedEvent?.Invoke(currentState);
    }
}
