using UnityEngine;

namespace InventoryScripts
{
    public class HeldItemManager
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

        public void ResetHeldItemPrefab() {
            heldItemPrefab = null;
        }
        public void DestroyHeldItemPrefab() {
            Object.Destroy(heldItemPrefab);
        }
    }
}