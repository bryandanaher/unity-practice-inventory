using System;

[Serializable]
public class ItemData
{
    public ItemSO ItemObject { get; set; }
    public string itemName;
    public int stackSize = 0;

    public ItemData(ItemSO item, int stack = 1) {
        ItemObject = item;
        itemName = item.displayName;
        stackSize = stack;
    }

    public void AddToStack() {
        stackSize++;
    }

    public void RemoveFromStack() {
        stackSize--;
    }
}