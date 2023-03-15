using System;
using UnityEngine;

public class CarnageController : MonoBehaviour {
    public static event Action<CarnageState> CarnageStateEvent;

    [SerializeField] private Rigidbody2D carnageRigidbody;
    [SerializeField] private float moveSpeed = 5f;
    // [SerializeField] private GameObject talkbox;
    
    private CarnageState carnageState = CarnageState.Squats;
    private bool facingRight = false;
    // private BoxCollider2D talkboxCollider;

    private void Awake() {
        AlleywayTextController.GumEvent += HandleGumEvent;
        CarnageAnimatorController.CarnageAnimationFinishedEvent += HandleAnimationFinished;

        // talkboxCollider = talkbox.GetComponent<BoxCollider2D>();
    }

    private void HandleGumEvent() {
        GetComponent<BoxCollider2D>().enabled = false;
        ChangeState(CarnageState.Leap);
    }

    private void HandleAnimationFinished(CarnageState state) {
        switch (state) {
            case CarnageState.Leap:
                ChangeState(CarnageState.SitDown);
                if (carnageRigidbody.constraints == RigidbodyConstraints2D.None) {
                    carnageRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                }
                GetComponent<BoxCollider2D>().enabled = true;
                break;
            case CarnageState.SitDown:
                ChangeState(CarnageState.Chewing);
                break;
        }
    }

    private void Update() {
        switch (carnageState) {
            case CarnageState.Squats:
                break;
            case CarnageState.Eat:
                break;
            case CarnageState.SitDown:
                break;
            case CarnageState.Chewing:
                break;
            case CarnageState.Leap:
                break;
        }
    }
    
    private void FixedUpdate() {
        if (carnageState == CarnageState.Leap) {
            if (!facingRight) {
                Flip();
            }
            if (carnageRigidbody.constraints == RigidbodyConstraints2D.FreezeAll) {
                carnageRigidbody.constraints = RigidbodyConstraints2D.None;
            }
            carnageRigidbody.velocity = new Vector2(1 * moveSpeed, -0.05f * moveSpeed);
        } else {
            if (facingRight) {
                Flip();
            }
            carnageRigidbody.velocity = new Vector2(0 * moveSpeed, 0 * moveSpeed);
        }
    }
    
    private void ChangeState(CarnageState newState) {
        carnageState = newState;
        CarnageStateEvent?.Invoke(carnageState);
    }

    private void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}