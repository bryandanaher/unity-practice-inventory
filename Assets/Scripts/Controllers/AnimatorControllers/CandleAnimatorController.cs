using System;
using UnityEngine;

public enum CandleState {
    Unlit,
    Lighting,
    Lit
}

public class CandleAnimatorController : MonoBehaviour {
    public static event Action<CandleState> CandleAnimationFinishedEvent;

    private Animator animator;
    private CandleState currentState = CandleState.Lighting;

    private void Awake() {
        CandleController.CandleStateEvent += ChangeAnimationState;
    }

    private void Start() {
        animator = GetComponent<Animator>();
    }

    private void ChangeAnimationState(CandleState newState) {
        if (currentState == newState) return;
        animator.Play(newState.ToString());
        currentState = newState;
    }

    private void AnimationFinished() {
        CandleAnimationFinishedEvent?.Invoke(currentState);
    }
}