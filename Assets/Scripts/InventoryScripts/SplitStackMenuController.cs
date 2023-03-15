using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SplitStackMenuController : MonoBehaviour
{
    public static event Action<GameObject> OnSplitStack;

    public TMP_Text percentageText;
    public Slider slider;
    public InventoryItem clickedInventoryItem;
    
    public void UpdateText() {
        percentageText.text = slider.value + "/" + slider.maxValue;
    }

    public void DestroyGameObject() {
        Destroy(gameObject);
    }

    public void SplitStackButtonClicked() {
        OnSplitStack?.Invoke(gameObject);
        DestroyGameObject();
    }
}
