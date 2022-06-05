using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<ItemDataSO> itemsInInventory;

    public void AddItemToInventory(ItemDataSO item)
    {
        itemsInInventory.Add(item);
    }
}
