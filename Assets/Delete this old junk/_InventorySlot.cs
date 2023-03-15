using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class _InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI labelText;
    public TextMeshProUGUI stackSizeText;

    public void ClearSlot() {
        icon.enabled = false;
        labelText.enabled = false;
        stackSizeText.enabled = false;
    }

    public void DrawSlot(_InventoryItem item) {
        if (item == null) {
            ClearSlot();
            return;
        }

        icon.enabled = true;
        labelText.enabled = true;
        stackSizeText.enabled = true;

        icon.sprite = item.itemData.ItemObject.icon;
        labelText.text = item.itemData.ItemObject.displayName;
        // stackSizeText.text = item.stackSize.ToString();
    }
}
