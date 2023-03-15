using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col) {
        var collectible = col.GetComponent<ICollectible>();
        if (collectible != null) {
            collectible.Collect();
        }
    }
}
