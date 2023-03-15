using System;
using UnityEngine;
using TMPro;

public class CoinText : MonoBehaviour {
    public TextMeshProUGUI coinText;
    private int coinCount = 0;

    private void OnEnable() {
        CollectibleItem.OnItemCollected += IncrementCoinCount;
    }
    
    private void OnDisable() {
        CollectibleItem.OnItemCollected += IncrementCoinCount;
    }

    public void IncrementCoinCount(ItemSO soData) {
        coinCount++;
        coinText.text = $"Coins: {coinCount}";
    }
}
