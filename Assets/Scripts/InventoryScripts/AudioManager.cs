using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    [SerializeField] private AudioSource coinSound, pickupSound, ohYeahSound;

    private void OnEnable() {
        CollectibleItem.OnItemCollected += PlayCoinSound;
    }

    private void OnDisable() {
        CollectibleItem.OnItemCollected -= PlayCoinSound;
    }
    public void PlayCoinSound(ItemSO data) {
        if (data.displayName == "Cool Sue Coin") {
            ohYeahSound.Play();
        } else {
            coinSound.Play();
        }
    }

}
