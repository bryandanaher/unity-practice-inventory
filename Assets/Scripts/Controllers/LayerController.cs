using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerController : MonoBehaviour {

    [SerializeField]
    private float yAxisOffset = 0;
    
    void FixedUpdate() {
        var position = transform.position;
        transform.position = new Vector3(position.x, position.y, position.y+yAxisOffset);
    }
}