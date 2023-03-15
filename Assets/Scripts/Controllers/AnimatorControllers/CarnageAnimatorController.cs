using System;
using UnityEngine;

public enum CarnageState {
    Eat,
    SitDown,
    Chewing,
    Squats,
    Leap
}

public class CarnageAnimatorController : MonoBehaviour {
    public static event Action<CarnageState> CarnageAnimationFinishedEvent;

    private Animator animator;
    private CarnageState currentState;
    
    private void Awake() {
        CarnageController.CarnageStateEvent += ChangeAnimationState;
    }
    
    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void ChangeAnimationState(CarnageState newState) {
        if (currentState == newState) return;
        animator.Play(newState.ToString());
        currentState = newState;
    }
    
    private void AnimationFinished() {
        CarnageAnimationFinishedEvent?.Invoke(currentState);
    }
}