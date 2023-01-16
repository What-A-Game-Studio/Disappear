using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WAG.Inventory_Items;
using Random = UnityEngine.Random;

namespace WAG.Items
{
    /// <summary>
    /// ItemManager 
    /// </summary>
    public class ItemManager : MonoBehaviour
    {
        public static ItemManager Instance { get; private set; }

        [SerializeField] private RarityTierSO[] RarityTiers;
        [SerializeField] private ItemDataSO[] itemsData;
        [SerializeField] private GameObject[] usablePrefabs;

        private ItemSpawner[] spawners;

        private int TotalItems, theoryItems;

        private int totalRate;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // if (PhotonNetwork.IsMasterClient)
            // {
            //     CreateItems();
            // }
        }

        /// <summary>
        /// Create all items in all spawns
        /// </summary>
        private void CreateItems()
        {
            totalRate = RarityTiers.Sum(tier => tier.Rate);
            spawners = GameObject.FindObjectsOfType<ItemSpawner>();

            foreach (ItemSpawner spawn in spawners)
            {
                // Debug.Log("Spawn Item Type : " + spawn.ItemType);
                ItemDataSO[] goodTypeItem = GetItemByType(spawn.ItemType);
                // Debug.Log("Item type count : " + goodTypeItem.Length);

                int nbItems = spawn.GetNbItemToSpawn();
                theoryItems += nbItems;
                for (int i = 0; i < nbItems; i++)
                {
                    ItemDataSO item = GetTierToSpawn(goodTypeItem);
                    if (item != null)
                    {
                        // pv.RPC(nameof(RPC_InstantiateItem),
                        //     RpcTarget.All,
                        //     spawn.SpawnCoordinate(),
                        //     Array.IndexOf(itemsData, item));
                        // ++TotalItems;
                    }
                }
            }
        }

        /// <summary>
        /// Find index of item in children
        /// </summary>
        /// <param name="item">item ton find</param>
        /// <returns>null if not found</returns>
        protected int? FindIndexOfItem(ItemController item)
        {
            int? indexInChildren = null;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.TryGetComponent(out ItemController ic) && ic == item)
                {
                    indexInChildren = i;
                    break;
                }
            }

            return indexInChildren;
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
                if (rte == item.TierEnum)
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

        /// <summary>
        /// Store item for inventory
        /// </summary>
        /// <param name="item">Item to store</param>
        public void StoreItem(ItemController item)
        {
            int? indexInChildren = FindIndexOfItem(item);

            if (!indexInChildren.HasValue)
                return;

            // pv.RPC(nameof(RPC_StoreItem),
            //     RpcTarget.All,
            //     indexInChildren.Value);
        }

        /// <summary>
        /// drop item from inventory
        /// </summary>
        /// <param name="item">Item to drop</param>
        /// <param name="position">Position to drop</param>
        /// <param name="forward">direction to drop</param>
        public void DropItem(ItemController item, Vector3 position, Vector3 forward)
        {
            int? indexInChildren = FindIndexOfItem(item);

            if (!indexInChildren.HasValue)
                return;
            // pv.RPC(nameof(RPC_DropItem),
            //     RpcTarget.All,
            //     indexInChildren.Value,
            //     position,
            //     forward);
        }

        /// <summary>
        /// Get one item by his position in array
        /// </summary>
        /// <param name="indexItem">item's index</param>
        /// <returns>return null if indexItem is greater than array lenght or less than 0</returns>
        public ItemDataSO GetItemById(int indexItem)
        {
            if (indexItem >= 0 && indexItem < itemsData.Length)
            {
                return itemsData[indexItem];
            }

            return null;
        }

        /// <summary>
        /// Find the prefab for usable item 
        /// </summary>
        /// <param name="item">The item to find</param>
        /// <returns>The usable item if it exists, null otherwise</returns>
        public GameObject GetUsable(ItemController item)
        {
            foreach (GameObject go in usablePrefabs)
            {
                string itemName = item.ItemData.ShortName.Split("_")[0];
                if (go.name.Contains(itemName))
                {
                    return go;
                }
            }

            return null;
        }

        #region ====================== Photon : Start ======================

 //       [PunRPC]
        private void RPC_StoreItem(int indexInChildren)
        {
            if (indexInChildren > transform.childCount)
                return;

            Transform child = transform.GetChild(indexInChildren);
            child.gameObject.SetActive(false);
            child.localPosition = Vector3.zero;
        }

  //      [PunRPC]
        private void RPC_DropItem(int indexInChildren, Vector3 spawnPos, Vector3 forwardOrientation)
        {
            if (indexInChildren > transform.childCount)
                return;

            Transform child = transform.GetChild(indexInChildren);
            child.GetComponent<ItemController>()?.Activate(spawnPos+Vector3.up, forwardOrientation);
        }

   //     [PunRPC] // Remote Procedure Calls
        protected virtual void RPC_InstantiateItem(Vector3 position, int itemToSpawn)
        {
            // Debug.Log("Item To Spawn : " + itemToSpawn);
            ItemDataSO item = GetItemById(itemToSpawn);
            if (item == null)
                return;

            GameObject go = Instantiate(item.Model, position, Quaternion.identity);
            go.transform.parent = transform;
            go.layer = LayerMask.NameToLayer("Interactable");
            ItemController ic = go.AddComponent<ItemController>();
            ic.ItemData = item;
        }

        #endregion ====================== Photon : End ======================
    }
}