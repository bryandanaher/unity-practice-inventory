using System;
using UnityEngine;

public class DoorAnimationHandler : MonoBehaviour {
    // public static event Action DoorOpenEvent;
    
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject doorBlocker;

    private bool canOpen = false;

    private void Awake() {
        PlayerInteractHandler.InteractEvent += PlayDoorAnimation;
        CarnageController.CarnageStateEvent += HandleCarnageSitting;
    }

    private void HandleCarnageSitting(CarnageState state) {
        if (state == CarnageState.SitDown) {
            canOpen = true;
        }
    }

    private void PlayDoorAnimation(GameObject go) {
        if (!canOpen || go.GetComponent<PlayerInteractHandler>().CurrentInteractTarget != PlayerInteractTarget.Door) {
            return;
        }
        animator.enabled = true;
        spriteRenderer.enabled = true;
    }

    internal void AnimationFinished() {
        animator.enabled = false;
        // DoorOpenEvent?.Invoke();
        doorBlocker.SetActive(false);
    }
}