using System;
using UnityEngine;

public class PickUpItem : Interactable
{
    private PlayerInventory inventory;
    public ItemController ItemController { get; set; }


    protected override void ActionOnInteract(GameObject sender)
    {
        if (sender.TryGetComponent<PlayerInventory>(out inventory))
        {
            inventory.AddItemToInventory(ItemController.ItemData);
            ItemManager.Instance.StoreItem(this);
        }
        else
        {
            Debug.LogError("Can't find player inventory");
        }
    }
}
