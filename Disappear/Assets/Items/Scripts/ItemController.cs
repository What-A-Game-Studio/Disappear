using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using WAG.Inventory_Items;
using WAG.Multiplayer;

namespace WAG.Items
{
    public class ItemController : NetworkSideBehaviour, IItemController
    {
        [SerializeField] private float forceAtSpawn = 1.2f;
        [SerializeField] private NetworkObject networkObject;
        [SerializeField] private Rigidbody rb;
        public float ForceAtSpawn => forceAtSpawn;

        [SerializeField] private float timeToCheckIfItemStill = 1f;
        [SerializeField] private ItemDataSO data;
        public ItemDataSO ItemData => data;

        public NetworkVariable<bool> IsActive = new NetworkVariable<bool>(true);


        private void Awake()
        {
            tag = "Interactable";
            IsActive.OnValueChanged += ValueChanged;
            ItemManager.OnInstantiate += (sender, args) =>
            {
                ItemManager.Instance.RegisterItem(
                    this,
                    NetworkObjectId);
            };
        }

        private void ValueChanged(bool previousvalue, bool newvalue)
        {
            gameObject.SetActive(newvalue);
        }


        private void Start()
        {
            if (ItemManager.Instance)
            {
                ItemManager.Instance.RegisterItem(
                    this,
                    NetworkObjectId);
            }
        }


        /// <summary>
        /// Reactivate item at main camera position
        ///  at forward direction
        /// </summary>
        /// <param name="spawnPos"></param>
        /// <param name="forwardOrientation"></param>
        public void Activate(Vector3 spawnPos, Vector3 forwardOrientation)
        {
            transform.position = spawnPos + forwardOrientation;
            rb.AddForce(forwardOrientation * forceAtSpawn * 2);
            gameObject.SetActive(true);
        }

        public void Drop(Vector3 position, Vector3 forward)
        {
            ItemManager.Instance.DropItem(this, position, forward);
        }

        public void Spawn()
        {
            GetComponent<NetworkObject>().Spawn(true);
        }
    }
}