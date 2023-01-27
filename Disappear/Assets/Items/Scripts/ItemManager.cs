using System;
using System.Collections.Generic;
using System.Linq;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;
using WAG.Inventory_Items;
using WAG.Multiplayer;
using Random = UnityEngine.Random;

namespace WAG.Items
{
    /// <summary>
    /// ItemManager 
    /// </summary>
    public class ItemManager : NetworkSideBehaviour
    {
        public static ItemManager Instance { get; private set; }

        public static event EventHandler OnInstantiate = delegate { };

        [SerializeField] private RarityTierSO[] RarityTiers;
        [SerializeField] private ItemController[] itemControllers;
        [SerializeField] private GameObject[] usablePrefabs;

        private ItemSpawner[] spawners;

        private int totalItems, theoryItems;

        private int totalRate;

        private Dictionary<ulong, ItemController> itemsRegistered = new Dictionary<ulong, ItemController>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                OnInstantiate.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        protected override void OnServerSpawn()
        {
            // NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManagerOnOnSceneEvent;
            CreateItems();
        }

        private void SceneManagerOnOnSceneEvent(SceneEvent sceneevent)
        {
            if (sceneevent.SceneEventType == SceneEventType.SynchronizeComplete)
            {
                CreateItems();
            }
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
                ItemController[] goodTypeItemControllers = GetItemByType(spawn.ItemType);

                int nbItems = spawn.GetNbItemToSpawn();
                theoryItems += nbItems;
                for (int i = 0; i < nbItems; i++)
                {
                    ItemController item = GetTierToSpawn(goodTypeItemControllers);
                    if (item != null)
                    {
                        InstantiateItem(spawn.SpawnCoordinate(), item.ItemData.Uid);
                        ++totalItems;
                    }
                }
            }
        }

        /// <summary>
        /// Get array of items of type passed in parameter
        /// </summary>
        /// <param name="it">Type of item to get</param>
        /// <returns>Array of itemSO</returns>
        protected ItemController[] GetItemByType(ItemType it)
        {
            return itemControllers.Where(data => data.ItemData.ItemType == it).ToArray();
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
        protected ItemController GetTierToSpawn(ItemController[] items)
        {
            RarityTiersEnum rte = GetTierToSpawn();
            List<ItemController> filteredItems = new List<ItemController>();
            foreach (ItemController item in items)
            {
                if (rte == item.ItemData.TierEnum)
                    filteredItems.Add(item);
            }

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
            StoreItemServerRpc(item.NetworkObjectId);
        }

        /// <summary>
        /// drop item from inventory
        /// </summary>
        /// <param name="item">Item to drop</param>
        /// <param name="position">Position to drop</param>
        /// <param name="forward">direction to drop</param>
        public void DropItem(ItemController item, Vector3 position, Vector3 forward)
        {
            // int? indexInChildren = FindIndexOfItem(item);

            // if (!indexInChildren.HasValue)
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
        /// <param name="idTosSpawn">item's id</param>
        /// <returns>return null if indexItem is greater than array lenght or less than 0</returns>
        public ItemController GetItemById(string idTosSpawn)
        {
            return itemControllers.FirstOrDefault(x => x.ItemData.Uid == idTosSpawn);
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

        [ServerRpc]
        private void StoreItemServerRpc(ulong indexItemToHide)
        {
            itemsRegistered[indexItemToHide].IsActive.Value = false;
        }


        private void RPC_DropItem(int indexInChildren, Vector3 spawnPos, Vector3 forwardOrientation)
        {
            if (indexInChildren > transform.childCount)
                return;

            Transform child = transform.GetChild(indexInChildren);
            child.GetComponent<ItemController>()?.Activate(spawnPos + Vector3.up, forwardOrientation);
        }

        protected virtual void InstantiateItem(Vector3 position, string itemId)

        {
            ItemController controller = GetItemById(itemId);
            if (controller == null)
            {
                return;
            }

            Transform itemToSpawn =
                Instantiate(controller.transform, position, Quaternion.identity, transform);
            itemToSpawn.GetComponent<ItemController>().Spawn();
        }

        private Vector3 GetRdmVector(float min, float max)
        {
            return new Vector3(Random.Range(min, max),
                Random.Range(min, max),
                Random.Range(min, max));
        }

        [Command]
        private void logRegisteredItem()
        {
            foreach (var ri in itemsRegistered)
            {
                Debug.Log("(" + ri.Key + ", " + ri.Value + ")");
            }
        }

        public void RegisterItem(ItemController itemController, ulong itemNetworkId)
        {
            Debug.Log(itemNetworkId, transform);
            itemsRegistered[itemNetworkId] = itemController;
        }
    }
}