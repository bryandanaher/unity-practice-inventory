using UnityEngine;
using System;

public class CollectibleItem : MonoBehaviour, ICollectible
{
    public static event Action<ItemSO> OnItemCollected;
    public ItemSO itemSO;
    
    public void Collect() {
        OnItemCollected?.Invoke(itemSO);
        Destroy(gameObject);
    }
}
