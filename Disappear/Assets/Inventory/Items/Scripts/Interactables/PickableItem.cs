using UnityEngine;
using WAG.Interactions;

namespace WAG.Inventory.Items
{
    public class PickableItem : Interactable
    {
        private InventoryController inventory;
        public ItemController ItemController { get; set; }


        protected override void ActionOnInteract(GameObject sender)
        {
            if (sender.TryGetComponent(out inventory))
            {
                if (inventory.AddItemToInventory(ItemController))
                    ItemManager.Instance.StoreItem(ItemController);
            }
            else
            {
                Debug.LogError("Can't find player inventory");
            }
        }
    }
}