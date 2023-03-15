using System;
using UnityEngine;

public class CandleController : MonoBehaviour {
    public static event Action<CandleState> CandleStateEvent;

    private CandleState candleState = CandleState.Lighting;

    private Transform[] candles;

    private void Awake() {
        DancePartyTextController.DialogueCompleteEvent += LightCandles;
        CandleAnimatorController.CandleAnimationFinishedEvent += HandleAnimationFinished;
        // LightCandlesEvent += LightCandles;
        candles = GetComponentsInChildren<Transform>();
    }

    private void Update() {
        switch (candleState) {
            case CandleState.Unlit:
                break;
            case CandleState.Lighting:
                break;
            case CandleState.Lit:
                break;
        }
    }

    private void LightCandles() {
        foreach (Transform candle in candles) {
            candle.gameObject.SetActive(true);
        }
    }

    private void HandleAnimationFinished(CandleState state) {
        if (state == CandleState.Lighting) {
            ChangeState(CandleState.Lit);
        }
    }

    private void ChangeState(CandleState newState) {
        candleState = newState;
        CandleStateEvent?.Invoke(candleState);
    }
}