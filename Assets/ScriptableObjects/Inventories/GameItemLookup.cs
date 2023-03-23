using System;
using UnityEngine;

//This assumes Item names are unique. May have to update this later
[CreateAssetMenu]
public class GameItemLookup : ScriptableObject
{
    public ItemSO[] gameItemArray = new ItemSO[20];

    public ItemSO FindItemByName(string itemName) {
        return Array.Find(gameItemArray, element => element.name == itemName);
    }
    
    public Sprite FindIconByName(string itemName) {
        return Array.Find(gameItemArray, element => element.name == itemName).icon;
    }
}
