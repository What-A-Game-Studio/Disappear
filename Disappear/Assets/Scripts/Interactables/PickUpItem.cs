using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PickUpItem : Item
{
    private PlayerInventory inventory;
    
    protected override void ActionOnInteract(GameObject sender)
    {
        if (sender.TryGetComponent(out inventory))
        {
            inventory.AddItemToInventory(this);
            PoolSystem.Instance.StoreInPool(this.gameObject.name);
        }
        else
        {
            Debug.LogError("Can't find player inventory");
        }
    }
}
