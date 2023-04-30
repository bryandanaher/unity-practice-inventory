using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NSubstitute;

namespace InventoryScripts
{
    public static class InventoryUIUtils
    {
        public static void SetGhostItem(GameObject itemObject) {
            itemObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
            itemObject.GetComponent<InventoryItem>().ghostItem = true;
        }
        
        public static GameObject CloneItem(GameObject itemPrefab, Transform parentTransform, ItemData itemData, int stackSize, (int start, int end) moveCoordinates) {
            var newItem = Object.Instantiate(itemPrefab, parentTransform);
            newItem.GetComponent<Image>().sprite = itemData.ItemObject.icon;
            newItem.GetComponentInChildren<TextMeshProUGUI>().text = stackSize.ToString();
            var newInventoryItem = newItem.GetComponent<InventoryItem>();
            newInventoryItem.parentAfterDrag = parentTransform;
            newInventoryItem.SetMoveCoordinates(moveCoordinates);
            return newItem;
        }
        
        public static bool OpenSplitMenu(GameObject clickedItem, GameObject splitModalPrefab, 
            Transform parentTransform, int stackSize, HeldItemManager him) {
            if (stackSize <= 1 || him.HoldingItem()) {
                return false;
            }

            var splitModal = CreateSplitModal(splitModalPrefab, parentTransform);
            var splitMenu = splitModal.transform.Find("SplitStackMenu");
            var itemIcon = splitMenu.transform.Find("ItemIcon").GetComponent<Image>();
            var slider = splitMenu.transform.Find("Slider").GetComponent<Slider>();
            var percentageText = splitMenu.transform.Find("PercentageText").GetComponent<TMP_Text>();

            splitModal.GetComponent<SplitStackMenuController>().clickedInventoryItem = clickedItem.GetComponent<InventoryItem>();
            itemIcon.sprite = clickedItem.GetComponent<InventoryItem>().image.sprite;
            slider.maxValue = stackSize;
            slider.value = stackSize - (stackSize / 2);
            percentageText.text = slider.value + "/" + stackSize;
            return true;
        }
        
        public static (int sliderValue, int sliderMaxValue) SplitModalSelectedValue(GameObject splitModal) {
            var splitMenu = splitModal.transform.Find("SplitStackMenu");
            var slider = splitMenu.transform.Find("Slider").GetComponent<Slider>();
            return ((int)slider.value, (int)slider.maxValue);
        }
        
        public static InventorySlotScript CreateInventorySlot(int index, GameObject slotPrefab, Transform parentTransform) {
            //The second parameter of Object.Instantiate sets the parent, instead of instantiating by default 
            //in a brand new transform hierarchy and then setting it to the proper parent afterwards
            var newSlot = Object.Instantiate(slotPrefab, parentTransform);
            var inventorySlotScript = newSlot.GetComponent<InventorySlotScript>();
            inventorySlotScript.id = index;
            return inventorySlotScript;
        }

        public static void PutInventoryItemInSlot(IInventoryItem inventoryItem, Transform parentAfterDrag, bool setCoordinates = true) {
            if (setCoordinates) {
                var newCoordinates = (inventoryItem.GetMoveCoordinates().end,
                    inventoryItem.GetMoveCoordinates().end);
                inventoryItem.SetMoveCoordinates(newCoordinates);
            }
            inventoryItem.SetParentAfterDrag(parentAfterDrag);
            inventoryItem.OnEndDrag();
        }

        public static GameObject CreateSplitModal(GameObject splitModalPrefab, Transform parentTransform) {
            var splitModal = Object.Instantiate(splitModalPrefab, parentTransform);
            var modalRt = splitModal.GetComponent<RectTransform>();
            modalRt.anchorMin = new Vector2(0, 0);
            modalRt.anchorMax = new Vector2(1, 1);
            modalRt.offsetMin = Vector2.zero;
            modalRt.offsetMax = Vector2.zero;
            return splitModal;
        }
    }
}