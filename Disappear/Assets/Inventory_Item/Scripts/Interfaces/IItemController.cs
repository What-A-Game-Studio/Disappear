using UnityEngine;

namespace WAG.Inventory_Items
{
    public interface IItemController
    {
        public ItemDataSO ItemData { get; }
        public void Drop(Vector3 position, Vector3 forward);
    }
}