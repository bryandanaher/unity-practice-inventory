using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInBounds : MonoBehaviour {
    [SerializeField] private BoxCollider2D boundBox;
    [SerializeField] private GameObject boundsCheck;

    // private BoxCollider2D groundCheck;
    private Vector3 minBounds;
    private Vector3 maxBounds;
    private float halfHeight;
    private float halfWidth;
    
    private void Start() {
        var boundBoxBounds = boundBox.bounds;
        minBounds = boundBoxBounds.min;
        maxBounds = boundBoxBounds.max;
        halfHeight = boundsCheck.GetComponent<BoxCollider2D>().size.y/2;
        halfWidth = boundsCheck.GetComponent<BoxCollider2D>().size.x/2;
    }

    private void Update() {
        // var startPosition = transform.position;
        // var playerPosition = player.transform.position;
        // playerPosition.x += positionOffset.x;
        // playerPosition.y += positionOffset.y;
        // playerPosition.z = -10;

        // transform.position = Vector3.Lerp(
            // cameraStartPosition, playerPosition, timeOffset * Time.deltaTime);
        // transform.position = Vector3.SmoothDamp(cameraStartPosition, playerPosition, ref velocity, timeOffset);

        var clampedX = Mathf.Clamp(
            boundsCheck.transform.position.x, minBounds.x + halfWidth, maxBounds.x - halfWidth);
        var clampedY = Mathf.Clamp(
            boundsCheck.transform.position.y, minBounds.y + halfHeight, maxBounds.y - halfHeight);
        boundsCheck.transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}