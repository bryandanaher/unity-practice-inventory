using UnityEngine;
using UnityEngine.UI;

public class _InventoryItemController : MonoBehaviour {
    private _ItemSO item;

    public Button RemoveButton;
    
    public void RemoveItem() {
        _InventoryManager.Instance.Remove(item);
        Destroy(gameObject);
    }
    
    public void AddItem(_ItemSO newItem) {
        item = newItem;
    }
}
