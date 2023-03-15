using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarnageTypeWriter : MonoBehaviour {
    public static event Action<bool, bool> CarnageTextFinishedEvent;
    
    [SerializeField] private GameObject nextButton;

    private TextMeshProUGUI displayText;
    // private string fullText;

    [SerializeField] private List<string> textList;
    private string gumText = "WOW, YOU HAVE GUM? I LOVE GUM! GIVE ME THAT!";
    
    private int currentString = 0;
    private bool typing = false;
    private bool finalTextString = false;

    private void Awake() {
        displayText = GetComponent<TextMeshProUGUI>();
    }

    public void HandleCarnageDialog(bool venomHasGum) {
        if (typing) return;
        
        if (venomHasGum) {
            finalTextString = true;
            StartCoroutine(TypeText(gumText));
        } else if (currentString >= textList.Count) {
            currentString = 0;
            finalTextString = true;
        } else {
            finalTextString = false;
            StartCoroutine(TypeText(textList[currentString]));
            currentString++;
        }
        CarnageTextFinishedEvent?.Invoke(finalTextString, venomHasGum);
    }

    private IEnumerator TypeText(string fullText) {
        nextButton.SetActive(false);
        typing = true;
        displayText.text = "";
        foreach (char c in fullText) {
            displayText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
        typing = false;
        nextButton.SetActive(true);
    }
}