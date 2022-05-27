using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [field:SerializeField] public ItemType ItemType { get; protected set; }
    [Range(1,10)]
    [SerializeField] private int maxItemsToSpawn = 1;
    private int? nbToSpawn = null;

    /// <summary>
    /// For the first call of this method we generate a random number [1:maxItemsToSpawn]
    /// who determinate how many items need to be instantiated
    /// </summary>
    /// <returns>Number of item to instantiate </returns>
    public int GetNbItemToSpawn()
    {
        if (nbToSpawn.HasValue)
            return nbToSpawn.Value;
        nbToSpawn = Random.Range(1, maxItemsToSpawn);
        Debug.Log(name +" need to spawn " + nbToSpawn);
        return nbToSpawn.Value;
    }

    public void InstantiateItem(ItemDataSO item)
    {
        Debug.Log(name + " InstantiateItem " + item.name);
    }
}
