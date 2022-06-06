using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PickableItem : Interactable
{
    private PlayerInventory inventory;
    public ItemController ItemController { get; set; }


    protected override void ActionOnInteract(GameObject sender)
    {
        if (sender.TryGetComponent(out inventory))
        {
            
            inventory.AddItemToInventory(ItemController);
            ItemManager.Instance.StoreItem(ItemController);
        }
        else
        {
            Debug.LogError("Can't find player inventory");
        }
    }
}
