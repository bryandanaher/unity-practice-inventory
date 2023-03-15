using UnityEngine;

public class _ItemPickup : MonoBehaviour {
    public _ItemSO item;

    private void Pickup() {
        _InventoryManager.Instance.Add(item);
        Destroy(gameObject);
    }

    //TODO: change this to something else
    private void OnMouseDown() {
        Pickup();
    }
}
