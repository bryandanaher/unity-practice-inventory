using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicDisplayer : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool keepHidden = false;

    public void ShowDisplayGraphic() {
        if (!keepHidden) {
            spriteRenderer.enabled = true;
        }
    }

    public void PlayDisplayGraphicAnimation() {
        if (keepHidden) {
            return;
        }
        animator.enabled = true;
        keepHidden = true;
    }

    public void HideDisplayGraphic() {
        animator.enabled = false;
        spriteRenderer.enabled = false;
    }
}