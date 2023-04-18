using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AlleywayTextController : MonoBehaviour {
    public static event Action<bool> TextBoxToggledEvent;
    public static event Action GumEvent;
    // public static event Action<GameObject> TextFinishedEvent;

    [SerializeField] private GameObject carnageDialogue;
    //TODO: Make this a list when needed as more text boxes appear
    [SerializeField] private GameObject tmpObject;
    
    // New Fields
    private TextWriter carnageTextWriter;
    [SerializeField] private GameObject nextButton;
    [SerializeField] private List<string> textList;
    private readonly string gumText = "WOW, YOU HAVE GUM? I LOVE GUM! GIVE ME THAT!";
    private int currentString = 0;

    // public bool FinalTextString => finalTextString;
    private bool finalTextString = false;

    private void Awake() {
        PlayerInteractHandler.InteractEvent += HandleInteractEvent;
        // CarnageTypeWriter.CarnageTextFinishedEvent += CarnageTextFinished;
        carnageTextWriter = tmpObject.GetComponent<TextWriter>();
    }

    private void HandleInteractEvent(GameObject interactingObject) {
        var interactTarget = interactingObject.GetComponent<PlayerInteractHandler>().CurrentInteractTarget;
        var venomGivesGum = interactingObject.GetComponent<PlayerController>().HasGum;
            
        if (interactingObject.name == "Player" && interactTarget == PlayerInteractTarget.Carnage) {
            carnageDialogue.SetActive(true);
            TextBoxToggledEvent?.Invoke(true);

            StartCoroutine(HandleCarnageDialog(venomGivesGum));

            if (finalTextString) {
                carnageDialogue.SetActive(false);
                finalTextString = false;
                TextBoxToggledEvent?.Invoke(false);
                if (venomGivesGum) {
                    GumEvent?.Invoke();
                }
            }
        }
    }

    private IEnumerator HandleCarnageDialog(bool venomHasGum) {
        if (carnageTextWriter.Typing) yield break;
    
        if (venomHasGum) {
            yield return StartCoroutine(carnageTextWriter.TypeText(gumText, nextButton));
            finalTextString = true;
        } else if (currentString >= textList.Count) {
            currentString = 0;
            finalTextString = true;
        } else {
            finalTextString = false;
            yield return StartCoroutine(
                carnageTextWriter.TypeText(textList[currentString], nextButton));
            currentString++;
        }
    }
}
