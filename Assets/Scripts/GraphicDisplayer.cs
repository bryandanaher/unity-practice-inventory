using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicDisplayer : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool keepHidden = false;

    internal void ShowDisplayGraphic() {
        if (!keepHidden) {
            spriteRenderer.enabled = true;
        }
    }
    
    internal void PlayDisplayGraphicAnimation() {
        if (!keepHidden) {
            animator.enabled = true;
            keepHidden = true;
        }
    }

    internal void HideDisplayGraphic() {
        animator.enabled = false;
        spriteRenderer.enabled = false;
    }
}
