using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class DancePartyTextController : MonoBehaviour {
    // public static event Action<bool> TextBoxToggledEvent;
    public static event Action DialogueCompleteEvent;

    [SerializeField] private GameObject spiderDialogue;

    //TODO: Make this a list when needed as more text boxes appear
    [SerializeField] private GameObject spiderTmpObject;
    [SerializeField] private TMP_FontAsset comicSans, spiderFont;

    private TextWriter spiderTextWriter;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private List<string> textList;
    private int currentString = 0;

    // public bool FinalTextString => finalTextString;
    // private bool finalTextString = false;

    private void Awake() {
        PlayerInteractHandler.InteractEvent += HandleInteractEvent;
        // CarnageTypeWriter.CarnageTextFinishedEvent += CarnageTextFinished;
        spiderTextWriter = spiderTmpObject.GetComponent<TextWriter>();
    }

    private void HandleInteractEvent(GameObject interactingObject) {
        var interactTarget = interactingObject.GetComponent<PlayerInteractHandler>().CurrentInteractTarget;

        if (interactingObject.name == "Player" && interactTarget == PlayerInteractTarget.DanceHall) {
            spiderDialogue.SetActive(true);
            StartCoroutine(HandleDialogue());
        }
    }

    private IEnumerator HandleDialogue() {
        if (spiderTextWriter.Typing) yield break;

        TMP_FontAsset fontToUse = null;
        float fontSize = 19.8f;
        TextAlignmentOptions alignmentOptions = TextAlignmentOptions.Top;

        if (currentString == 5) {
            fontToUse = spiderFont;
            fontSize = 36f;
            alignmentOptions = TextAlignmentOptions.Midline;
        } else {
            fontToUse = comicSans;
        }

        if (currentString == textList.Count) {
            DialogueCompleteEvent?.Invoke();
            // finalTextString = true;
        } else if(currentString < textList.Count) {
            // finalTextString = false;
            yield return StartCoroutine(
                spiderTextWriter.TypeText(
                    textList[currentString], nextButton, fontToUse, fontSize, alignmentOptions));
            currentString++;
        }
    }
}