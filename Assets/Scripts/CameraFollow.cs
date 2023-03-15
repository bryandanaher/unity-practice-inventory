using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private GameObject player;
    [SerializeField] private Vector2 positionOffset;
    [SerializeField] private float timeOffset;
    // [SerializeField] private GameObject background;
    [SerializeField] private BoxCollider2D boundBox;
    private Vector3 minBounds;
    private Vector3 maxBounds;
    private Camera mainCamera;
    private float halfHeight;
    private float halfWidth;

    private bool cameraLock { get; set; }
    
    // //SmoothDamp will update this property apparently...
    // private Vector3 velocity;

    private void Awake() {
        PlayerInteractHandler.InteractEvent += HandleInteractEvent;
    }
    
    private void Start() {
        var boundBoxBounds = boundBox.bounds;
        minBounds = boundBoxBounds.min;
        maxBounds = boundBoxBounds.max;
        mainCamera = GetComponent<Camera>();
        halfHeight = mainCamera.orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height;
    }

    private void Update() {
        if (!cameraLock) {
            var cameraStartPosition = transform.position;
            var playerPosition = player.transform.position;
            playerPosition.x += positionOffset.x;
            playerPosition.y += positionOffset.y;
            playerPosition.z = -10;

            transform.position = Vector3.Lerp(
                cameraStartPosition, playerPosition, timeOffset * Time.deltaTime);
            // transform.position = Vector3.SmoothDamp(cameraStartPosition, playerPosition, ref velocity, timeOffset);

            var clampedX = Mathf.Clamp(
                transform.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
            var clampedY = Mathf.Clamp(
                transform.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);
            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }

    private void HandleInteractEvent(GameObject interactingObject) {
        var interactTarget = interactingObject.GetComponent<PlayerInteractHandler>().CurrentInteractTarget;

        if (interactTarget == PlayerInteractTarget.DanceHall) {
            cameraLock = true;
        }
    }
}