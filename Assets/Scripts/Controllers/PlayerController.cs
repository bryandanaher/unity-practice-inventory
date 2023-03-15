using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public static event Action<VenomState> VenomStateEvent;

    [SerializeField] private PlayerInteractHandler interactHandler;
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 9f;

    private VenomState currentState = VenomState.Idle;
    
    public bool HasGum => hasGum;
    private bool hasGum; //Next I should do inventory systems

    // PlayerInputActions script is generated from the PlayerInputActions inspector
    private InputAction move;
    // private InputAction fire;
    private InputAction run;
    private InputAction jump;
    private InputAction reach;
    private InputAction interact;

    private Vector2 moveDirection = Vector2.zero;
    private bool facingRight = true;
    private bool moving;

    private bool running;

    private bool venomIsReading = false;

    private void OnEnable() {
        VenomAnimatorController.AnimationFinishedEvent += AnimationFinished;
        AlleywayTextController.TextBoxToggledEvent += HandleTextBoxToggled;
        InitializeInputActions();
    }

    private void OnDisable() {
        VenomAnimatorController.AnimationFinishedEvent -= AnimationFinished;
        AlleywayTextController.TextBoxToggledEvent -= HandleTextBoxToggled;
        // CleanUpInputActions();
    }

    private void Jump(InputAction.CallbackContext context) {
        ChangeState(VenomState.Jump);
    }

    private void Reach(InputAction.CallbackContext context) {
        if (currentState != VenomState.Jump) {
            ChangeState(VenomState.Reach);
        }
    }

    private void AnimationFinished(VenomState animationState, bool venomHasGum) {
        if (animationState is VenomState.Jump or VenomState.Reach) {
            ChangeState(VenomState.Idle);
        }
        if (!hasGum) { //Don't want to turn off gum if we already have it
            hasGum = venomHasGum;
        }
    }

    void Update() {
        moveDirection = move.ReadValue<Vector2>();
        moving = Mathf.Abs(moveDirection.x) > 0 || Mathf.Abs(moveDirection.y) > 0;

        switch (currentState) {
            case VenomState.Idle:
                if (moving) {
                    ChangeState(running ? VenomState.Run : VenomState.Walk);
                }
                break;
            case VenomState.Walk:
                if (!moving) {
                    ChangeState(VenomState.Idle);
                } else if (running) {
                    ChangeState(VenomState.Run);
                }
                break;
            case VenomState.Run:
                if (!moving) {
                    ChangeState(VenomState.Idle);
                } else if (!running) {
                    ChangeState(VenomState.Walk);
                }
                break;
        }
    }

    private void FixedUpdate() {
        if (currentState == VenomState.Reach || venomIsReading) {
            playerRigidbody.velocity = new Vector2(0, 0);
        } else if (running) {
            playerRigidbody.velocity = new Vector2(
                moveDirection.x * runSpeed, moveDirection.y * runSpeed);
        } else {
            playerRigidbody.velocity = new Vector2(
                moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
        }

        if ((moveDirection.x > 0 && !facingRight) ||
            (moveDirection.x < 0 && facingRight)) {
            Flip();
        }
    }

    private void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void ChangeState(VenomState newState) {
        currentState = newState;
        VenomStateEvent?.Invoke(currentState);
    }

    // private void Fire(InputAction.CallbackContext context) {
    //     Debug.Log("We fired");
    // }

    private void ToggleRunning(InputAction.CallbackContext context) {
        running = !running;
    }

    private void ToggleChewingGum(InputAction.CallbackContext context) {
        hasGum = !hasGum;
    }

    private void HandleTextBoxToggled(bool textBoxOpen) {
        venomIsReading = textBoxOpen;
    }

    private void Interact(InputAction.CallbackContext context) {
        interactHandler.Interact();
    }

    private void InitializeInputActions() {
        move = InputManager.inputActions.Player.Move;
        // move.Enable();

        // fire = playerControls.Player.Fire;
        // fire.Enable();
        // fire.performed += Fire;

        run = InputManager.inputActions.Player.Run;
        // run.Enable();
        run.started += ToggleRunning;
        run.canceled += ToggleRunning;

        jump = InputManager.inputActions.Player.Jump;
        // jump.Enable();
        jump.started += Jump;

        reach = InputManager.inputActions.Player.Reach;
        // reach.Enable();
        reach.started += Reach;

        interact = InputManager.inputActions.Player.Interact;
        // interact.Enable();
        interact.started += Interact;
    }

    //TODO: Make sure these are properly disabled by the InputManager
    // private void CleanUpInputActions() {
    //     move.Disable();
    //     // fire.Disable();
    //     run.Disable();
    //     jump.Disable();
    //     reach.Disable();
    //     interact.Disable();
    // }
}