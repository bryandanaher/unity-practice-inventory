using UnityEngine;

public class InventoryController : MonoBehaviour {
    // public InventoryObject inventory;
    
    

    public void OnTriggerEnter2D(Collider2D col) {
        // Debug.Log("Code reached");
        // var item = col.GetComponent<Item>();
        // if(item) {
        //     inventory.AddItem(item.item, 1);
        //     Destroy(col.gameObject);
        // }
    }

    private void OnApplicationQuit() {
        // inventory.container.Clear();
    }
}
