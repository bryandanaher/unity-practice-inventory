using System.Collections;
using InventoryScripts;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class InventoryUIUtilsTest
    {
        // public static GameObject CloneItem(GameObject itemPrefab,
        // Transform parentTransform, ItemData itemData, int stackSize,
        // (int start, int end) moveCoordinates) {
        //     var newItem = Object.Instantiate(itemPrefab, parentTransform);
        //     newItem.GetComponent<Image>().sprite = itemData.ItemObject.icon;
        //     newItem.GetComponentInChildren<TextMeshProUGUI>().text = stackSize.ToString();
        //     var newInventoryItem = newItem.GetComponent<InventoryItem>();
        //     newInventoryItem.parentAfterDrag = parentTransform;
        //     newInventoryItem.SetMoveCoordinates(moveCoordinates);
        //     return newItem;
        // }
        // A Test behaves as an ordinary method
        [Test]
        public void CloneItem_ReturnsGameObject() {
            var itemPrefab = Object.Instantiate(Resources.Load<GameObject>("DragAndDrop/Item"));
            var parentTransform = new GameObject().transform;
            var itemData = new ItemData(ScriptableObject.CreateInstance<ItemSO>());
            (int start, int end) moveCoordinates = (2, 5);
            
            var actual = InventoryUIUtils.CloneItem(itemPrefab, parentTransform, 
                itemData, 2, moveCoordinates);
            Assert.NotNull(actual);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
