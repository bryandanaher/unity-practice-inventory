using System;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerClickHandler, IInventoryItem
{
    public static event Action<GameObject> OnItemClicked;
    public static event Action<GameObject> OnItemShiftClick;

    public Image image;
    public bool ghostItem;

    [HideInInspector] public Transform parentAfterDrag;

    private (int start, int end) moveCoordinates;
    private bool shifting = false;
    
    //TODO: the UIManager has to keep track of this too so we can't pick up multiple items at once
    //or maybe just have logic that tries to switch them out with each other...
    //see if we can get rid of this variable
    private bool dragging = false;

    private InputAction leftClick;
    private InputAction shift;

    private Camera mainCamera;

    private void OnEnable() {
        InitializeInputActions();
        mainCamera = Camera.main;
    }

    private void Update() {
        if (dragging) {
            OnDrag();
        }
    }

    public void OnBeginDrag() {
        if (dragging) {
            dragging = false;
            return;
        }
        parentAfterDrag = transform.parent;
        moveCoordinates.start = parentAfterDrag.GetComponent<InventorySlotScript>().id;
        //The parent has to be a canvas, or this will make the item disappear instead of 
        //appearing on top of everything.
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        //This makes the mouse pretend that the image doesn't exist, so the pointer can detect 
        //the inventory slot below the inventory item object
        image.raycastTarget = false;
        dragging = true;
    }

    private void OnDrag() {
        //This has to be 5 because it automatically sets the z value to -387
        //and adds about 77.6 for each 1f I add to the z value. Does that make sense? 
        var dragPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        dragPosition.z = mainCamera.transform.position.z + 5f;
        transform.position = dragPosition;
    }

    public void OnEndDrag() {
        //When a slot is clicked, the UIManager sets parentAfterDrag to the clicked slot
        moveCoordinates.end = parentAfterDrag.GetComponent<InventorySlotScript>().id;
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        dragging = false;
    }

    public void OnPointerClick (PointerEventData eventData) {
        if (shifting) {
            OnItemShiftClick?.Invoke(transform.gameObject);
        } else {
            OnItemClicked?.Invoke(transform.gameObject);
        }
    }

    private void ToggleShifting(InputAction.CallbackContext context) {
        shifting = true;
    }
    private void ShiftCancelled(InputAction.CallbackContext context) {
        shifting = false;
    }

    private void InitializeInputActions() {
        shift = InputManager.inputActions.UI.Shift;
        shift.started += ToggleShifting;
        shift.canceled += ShiftCancelled;
    }

    public (int start, int end) GetMoveCoordinates() {
        return moveCoordinates;
    }
    
    public void SetMoveCoordinates((int start, int end) inputCoordinates) {
        moveCoordinates = inputCoordinates;
    }

    public void SetParentAfterDrag(Transform newParent) {
        parentAfterDrag = newParent;
    }
}
