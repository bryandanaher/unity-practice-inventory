using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventorySlotScript : MonoBehaviour, IPointerClickHandler
{
    public static event Action<int, bool> OnSlotClicked;
    
    public GameObject itemPrefab;
    [HideInInspector] public int id;
    
    //TODO: move this into a parent object so every slot doesn't have to keep track of this object
    // private DraggableItem draggingItem;
    private InputAction leftClick;
    

    public bool EmptySlot() {
        return transform.childCount == 0;
    }

    private void ClearSlot() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }  
    }

    public void DrawSlot(ItemData itemData) {
        ClearSlot();
        var newItem = Instantiate(itemPrefab, transform);
        newItem.GetComponent<Image>().sprite = itemData.ItemObject.icon;
        newItem.GetComponentInChildren<TextMeshProUGUI>().text = itemData.stackSize.ToString();
        var inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.SetMoveCoordinates((id, id));
    }
    
    public void OnPointerClick (PointerEventData eventData) {
        if (transform.childCount != 0) {
            return;
        }
        OnSlotClicked?.Invoke(id, true);
    }
}
