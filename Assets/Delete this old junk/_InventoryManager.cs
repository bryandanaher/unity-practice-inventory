using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class _InventoryManager : MonoBehaviour {
    public static _InventoryManager Instance;
    public List<_ItemSO> items = new List<_ItemSO>();

    public Transform itemContent;
    public GameObject itemPrefab;

    public _InventoryItemController[] inventoryItems;

    void Awake() {
        // TODO: What is this trash
        Instance = this;
    }

    public void Add(_ItemSO item) {
        items.Add(item);
    }
    
    public void Remove(_ItemSO item) {
        items.Remove(item);
    }

    public void ListItems() {
        foreach (Transform item in itemContent) {
            Destroy(item.gameObject);
        }
        foreach (var item in items) {
            GameObject obj = Instantiate(itemPrefab, itemContent);
            // These are empty, we're just getting a reference to them apparently
            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var removeButton = obj.transform.Find("RemoveButton").GetComponent<Button>();
            
            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;
            removeButton.gameObject.SetActive(true);
        }

        SetInventoryItems();
    }

    public void SetInventoryItems() {
        inventoryItems = itemContent.GetComponentsInChildren<_InventoryItemController>();
        for (int i = 0; i < items.Count; i++) {
            inventoryItems[i].AddItem(items[i]);
        }
    }
}
