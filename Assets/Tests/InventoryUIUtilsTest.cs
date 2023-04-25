using System.Collections;
using InventoryScripts;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

// A [Test] behaves as an ordinary method
// A [UnityTest] behaves like a coroutine in Play Mode. In Edit Mode you can use
// `yield return null;` to skip a frame.
namespace Tests
{
    public class InventoryUIUtilsTest
    {
        [Test]
        public void SetGhostItem_Test() {
            //Setup
            var itemPrefab = Object.Instantiate(Resources.Load<GameObject>("DragAndDrop/Item"));

            //Validate initial state
            Assert.AreEqual(new Color(1,1,1,1), itemPrefab.GetComponent<Image>().color);
            Assert.False(itemPrefab.GetComponent<InventoryItem>().ghostItem);

            //Test
            InventoryUIUtils.SetGhostItem(itemPrefab);
            Assert.AreEqual(new Color(1,1,1,0.4f), itemPrefab.GetComponent<Image>().color);
            Assert.True(itemPrefab.GetComponent<InventoryItem>().ghostItem);
        }

        [Test]
        public void CloneItem_ReturnsGameObject() {
            //Setup
            var itemPrefab = Object.Instantiate(Resources.Load<GameObject>("DragAndDrop/Item"));
            var parentTransform = new GameObject().transform;
            var itemData = new ItemData(ScriptableObject.CreateInstance<ItemSO>());
            (int start, int end) moveCoordinates = (2, 5);
            
            //Test
            var actual = InventoryUIUtils.CloneItem(itemPrefab, parentTransform, 
                itemData, 2, moveCoordinates);
            Assert.NotNull(actual);
        }

        [Test]
        public void OpenSplitMenu_StackTooSmallToSplit() {
            //Setup
            var itemPrefab = Object.Instantiate(Resources.Load<GameObject>("DragAndDrop/Item"));
            var splitModalPrefab = Object.Instantiate(Resources.Load<GameObject>("SplitStackModal"));
            var parentTransform = new GameObject().transform;
            var heldItemManager = new HeldItemManager();

            //Test
            bool result = InventoryUIUtils.OpenSplitMenu(itemPrefab, splitModalPrefab, parentTransform, 1, heldItemManager);
            Assert.False(result);
        }

        [Test]
        public void OpenSplitMenu_AlreadyHoldingItem() {
            //Setup
            var itemPrefab = Object.Instantiate(Resources.Load<GameObject>("DragAndDrop/Item"));
            var splitModalPrefab = Object.Instantiate(Resources.Load<GameObject>("SplitStackModal"));
            var parentTransform = new GameObject().transform;
            var heldItemManager = new HeldItemManager();
            heldItemManager.SetHeldItemPrefab(itemPrefab);

            //Test
            bool result = InventoryUIUtils.OpenSplitMenu(itemPrefab, splitModalPrefab, parentTransform, 4, heldItemManager);
            Assert.False(result);
        }

        [Test]
        public void OpenSplitMenu_CanSplit() {
            //Setup
            var itemPrefab = Object.Instantiate(Resources.Load<GameObject>("DragAndDrop/Item"));
            var splitModalPrefab = Object.Instantiate(Resources.Load<GameObject>("SplitStackModal"));
            var parentTransform = new GameObject().transform;
            var heldItemManager = new HeldItemManager();

            //Test
            bool result = InventoryUIUtils.OpenSplitMenu(itemPrefab, splitModalPrefab, parentTransform, 4, heldItemManager);
            Assert.True(result);
        }

        [Test]
        public void SplitModalSelectedValue_Test() {
            //Setup
            var splitModalPrefab = Object.Instantiate(Resources.Load<GameObject>("SplitStackModal"));
            var parentTransform = new GameObject().transform;
            var splitModal = InventoryUIUtils.CreateSplitModal(splitModalPrefab, parentTransform);
            var slider = splitModal.transform.Find("SplitStackMenu").transform.Find("Slider").GetComponent<Slider>();
            slider.maxValue = 100;
            slider.value = 25;

            //Test
            (int sliderValue, int maxValue) result = InventoryUIUtils.SplitModalSelectedValue(splitModal);
            Assert.AreEqual((25,100), result);
        }

        [Test]
        public void CreateInventorySlot_Test() {
            //Setup
            var parentTransform = new GameObject().transform;
            var slotPrefab = Object.Instantiate(Resources.Load<GameObject>("DragAndDrop/InventorySlot"));

            //Test
            var result = InventoryUIUtils.CreateInventorySlot(4, slotPrefab, parentTransform);
            Assert.NotNull(result);
            Assert.IsInstanceOf<InventorySlotScript>(result);
        }

        [Test]
        public void PutInventoryItemInSlot_Test() {

        }

        [Test]
        public void CreateSplitModal_Test() {

        }


        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
