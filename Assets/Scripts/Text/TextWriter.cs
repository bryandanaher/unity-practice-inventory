using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;

public class TextWriter : MonoBehaviour {
    private TextMeshProUGUI displayText;

    public bool Typing => typing;
    private bool typing = false;

    private void Awake() {
        displayText = GetComponent<TextMeshProUGUI>();
    }

    public IEnumerator TypeText(
        string fullText, GameObject nextButton = null, TMP_FontAsset newFont = null, 
        float fontSize = 0, TextAlignmentOptions? alignment = null) {
        nextButton?.SetActive(false);
        if (newFont != null && newFont != displayText.font) {
            displayText.font = newFont;
        }
        if (fontSize != 0) {
            displayText.fontSize = fontSize;
        }
        if (alignment != null) {
            displayText.alignment = alignment ?? TextAlignmentOptions.Top;
        }
        
        typing = true;
        displayText.text = "";
        foreach (char c in fullText) {
            displayText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        typing = false;
        nextButton?.SetActive(true);
    }
}