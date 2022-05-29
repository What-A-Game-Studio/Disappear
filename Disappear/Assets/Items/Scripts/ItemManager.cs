using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
/// <summary>
/// ItemManager 
/// </summary>
public class ItemManager : MonoBehaviour
{
    
    public static ItemManager Instance { get; private set; }
    
    [SerializeField] private RarityTierSO[] RarityTiers;
    [SerializeField] private ItemDataSO[] itemsData;
    [Header("DEBUG")]
    [SerializeField] private ItemSpawner[] spawners;

    private int TotalItems, theoryItems;

    private int totalRate;
    private void Awake()
    {		
        if(Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        
        Debug.Log("ItemManager awaked");
        
        totalRate = RarityTiers.Sum(tier => tier.Rate);
        spawners = GameObject.FindObjectsOfType<ItemSpawner>();
        foreach (ItemSpawner spawn in spawners)
        {
            ItemDataSO[] goodTypeItem = GetItemByType(spawn.ItemType);
            int nbItems = spawn.GetNbItemToSpawn();
            theoryItems += nbItems;
            for (int i = 0; i < nbItems; i++)
            {
                ItemDataSO item = GetTierToSpawn(goodTypeItem);
                if (item != null)
                {
                    spawn.InstantiateItem(item);
                    ++TotalItems;
                }
            }
        }
        Debug.Log(TotalItems + " / " + theoryItems);
    }

    /// <summary>
    /// Get array of items of type passed in parameter
    /// </summary>
    /// <param name="it">Type of item to get</param>
    /// <returns>Array of itemSO</returns>
    protected ItemDataSO[] GetItemByType(ItemType it)
    {
        return itemsData.Where(data => data.ItemType == it).ToArray();
    }

    /// <summary>
    /// Return RarityTiersEnum related to rdm number 
    /// </summary>
    /// <returns>Rarity</returns>
    protected RarityTiersEnum GetTierToSpawn()
    {
        float p = Random.Range(0f, totalRate);
        int runningTotal = 0;
        foreach (RarityTierSO rt in RarityTiers)
        {
            runningTotal += rt.Rate;
            if (p < runningTotal)
                return rt.Rarity;
        }
        return RarityTiersEnum.Common;
    }

    /// <summary>
    /// Return ItemDataSO related to RarityTiersEnum generate by GetTierToSpawn()
    /// in array passed in parameter
    /// </summary>
    /// <param name="items"></param>
    /// <returns>Null if none of the items corresponds to the rarity generated</returns>
    protected ItemDataSO GetTierToSpawn(ItemDataSO[] items)
    {
        RarityTiersEnum rte = GetTierToSpawn();
        List<ItemDataSO> filteredItems = new List<ItemDataSO>();
        foreach (ItemDataSO item in items)
        {
            if(rte == item.TierEnum)
                filteredItems.Add(item);
        }
        // ItemDataSO[] filteredTiers = items.Where(x => x.TierEnum == rte).ToArray();
        if (filteredItems.Count > 0)
        {
            int idx = Random.Range(0, filteredItems.Count);
            return filteredItems[idx];
        }
        return null;
    }

}