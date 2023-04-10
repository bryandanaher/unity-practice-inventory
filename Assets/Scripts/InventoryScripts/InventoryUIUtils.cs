using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace InventoryScripts
{
    public class InventoryUIUtils
    {
        private GameObject heldItemPrefab;

        public bool HoldingItem() {
            return heldItemPrefab != null;
        }

        public GameObject GetHeldItem() {
            return heldItemPrefab;
        }

        public void SetHeldItemPrefab(GameObject prefab) {
            heldItemPrefab = prefab;
        }

        public int GetHeldItemCoordinatesStart() {
            return heldItemPrefab.GetComponent<InventoryItem>().GetMoveCoordinates().start;
        }
        
        public int GetHeldItemCoordinatesEnd() {
            return heldItemPrefab.GetComponent<InventoryItem>().GetMoveCoordinates().end;
        }

        public InventoryItem GetHeldInventoryItem() {
            return heldItemPrefab.GetComponent<InventoryItem>();
        }

        public void DestroyHeldItemPrefab() {
            Object.Destroy(heldItemPrefab);
        }
        
        public GameObject CloneItem(GameObject itemPrefab, Transform parentTransform, ItemData itemData, int stackSize, (int start, int end) moveCoordinates) {
            var newItem = Object.Instantiate(itemPrefab, parentTransform);
            newItem.GetComponent<Image>().sprite = itemData.ItemObject.icon;
            newItem.GetComponentInChildren<TextMeshProUGUI>().text = stackSize.ToString();
            var newInventoryItem = newItem.GetComponent<InventoryItem>();
            newInventoryItem.parentAfterDrag = parentTransform;
            newInventoryItem.SetMoveCoordinates(moveCoordinates);
            return newItem;
        }
    }
}