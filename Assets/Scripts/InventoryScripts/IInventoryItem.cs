using UnityEngine;

public interface IInventoryItem
{
    public void OnBeginDrag();
    public void OnEndDrag();
    public (int start, int end) GetMoveCoordinates();
    public void SetMoveCoordinates((int start, int end) inputCoordinates);
    public void SetParentAfterDrag(Transform newParent);
}
