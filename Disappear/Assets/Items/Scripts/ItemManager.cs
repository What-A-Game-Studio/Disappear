using System;
using System.Collections.Generic;
using System.Linq;
using QFSW.QC;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using WAG.Inventory_Items;
using WAG.Multiplayer;
using Random = UnityEngine.Random;

namespace WAG.Items
{
    public struct ItemNetworkData : INetworkSerializable, IEquatable<ItemNetworkData>
    {
        public Vector3 Position;
        public FixedString64Bytes ItemToSpawn;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref ItemToSpawn);
        }

        public bool Equals(ItemNetworkData other)
        {
            return other.ItemToSpawn == this.ItemToSpawn && other.Position == Position;
        }
    }

    /// <summary>
    /// ItemManager 
    /// </summary>
    public class ItemManager : NetworkSideBehaviour
    {
        private NetworkList<ItemNetworkData> ItemPool;
        private List<ItemNetworkData> localPool = new List<ItemNetworkData>();

        public static ItemManager Instance { get; private set; }

        [SerializeField] private RarityTierSO[] RarityTiers;
        [SerializeField] private ItemController[] itemControllers;
        [SerializeField] private GameObject[] usablePrefabs;

        private ItemSpawner[] spawners;

        private int TotalItems, theoryItems;

        private int totalRate;

        private void Awake()
        {
            Debug.Log("Item Manager Awake", this);
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            ItemPool = new NetworkList<ItemNetworkData>(new List<ItemNetworkData>());
            if (!IsServer)
            {
                Debug.Log("Client sub to itempool", this);
                ItemPool.OnListChanged += ItemPoolOnOnListChanged;
            }
        }

        protected override void OnClientSpawn()
        {
            // foreach (ItemNetworkData item in ItemPool)
            //     if (!localPool.Any(x => x.Equals(item)))
            //         InstantiateItem(item);
        }

        protected override void OnDespawn()
        {
            ItemPool.Dispose();
        }

        private void ItemPoolOnOnListChanged(NetworkListEvent<ItemNetworkData> changeevent)
        {
            // Debug.Log(
            //     changeevent.Type + " " + changeevent.Index + " (" + changeevent.Value.ItemToSpawn + ", " +
            //     changeevent.Value.Position + ")", this);
            // InstantiateItem(changeevent.Value);
        }

        protected override void OnServerSpawn()
        {
            Debug.Log("item Manager spawn", this);
            CreateItems();
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
                ItemController[] goodTypeItemControllers = GetItemByType(spawn.ItemType);
                // Debug.Log("Item type count : " + goodTypeItem.Length);

                int nbItems = spawn.GetNbItemToSpawn();
                theoryItems += nbItems;
                for (int i = 0; i < nbItems; i++)
                {
                    ItemController item = GetTierToSpawn(goodTypeItemControllers);
                    if (item != null)
                    {
                        // ItemPool.Add(Array.IndexOf(itemsData, item));
                        var tmp = new ItemNetworkData()
                        {
                            Position = spawn.SpawnCoordinate(),
                            ItemToSpawn = item.ItemData.Uid
                        };
                        ItemPool.Add(tmp);
                        InstantiateItem(tmp);
                        ++TotalItems;
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
        /// <param name="idTosSpawn">item's id</param>
        /// <returns>return null if indexItem is greater than array lenght or less than 0</returns>
        public ItemController GetItemById(string idTosSpawn)
        {
            Debug.Log(idTosSpawn, this);
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


        private void RPC_StoreItem(int indexInChildren)
        {
            if (indexInChildren > transform.childCount)
                return;

            Transform child = transform.GetChild(indexInChildren);
            child.gameObject.SetActive(false);
            child.localPosition = Vector3.zero;
        }


        private void RPC_DropItem(int indexInChildren, Vector3 spawnPos, Vector3 forwardOrientation)
        {
            if (indexInChildren > transform.childCount)
                return;

            Transform child = transform.GetChild(indexInChildren);
            child.GetComponent<ItemController>()?.Activate(spawnPos + Vector3.up, forwardOrientation);
        }

        protected virtual void InstantiateItem(ItemNetworkData networkItem)

        {
            // Debug.Log("Item To Spawn : " + itemToSpawn);
            ItemController controller = GetItemById(networkItem.ItemToSpawn.Value);
            if (controller == null)
            {
                Debug.Log("Fail to Instantiate item ", this);
                return;
            }

            Transform itemToSpawn =
                Instantiate(controller.transform, networkItem.Position, Quaternion.identity, transform);
            itemToSpawn.GetComponent<ItemController>().Spawn();
        }

        private Vector3 GetRdmVector(float min, float max)
        {
            return new Vector3(Random.Range(min, max),
                Random.Range(min, max),
                Random.Range(min, max));
        }

        [Command]
        private void LogNetworkPoolItems()
        {
            foreach (var item in ItemPool)
            {
                Debug.Log(
                    item.ItemToSpawn.Value + ", " +
                    item.Position, this);
            }
        }

        [Command]
        private void LogLocalPoolItems()
        {
            foreach (var item in ItemPool)
            {
                Debug.Log(
                    item.ItemToSpawn.Value + ", " +
                    item.Position, this);
            }
        }
    }
}