using System.Collections;
using Unity.Netcode;
using UnityEngine;
using WAG.Inventory_Items;
using WAG.Multiplayer;
using Random = UnityEngine.Random;

namespace WAG.Items
{
    public class ItemController : NetworkSideBehaviour, IItemController
    {
        [SerializeField] private float forceAtSpawn = 1.2f;
        public float ForceAtSpawn => forceAtSpawn;
        private Rigidbody rb;

        [SerializeField] private float timeToCheckIfItemStill = 1f;
        [SerializeField] private ItemDataSO data;
        public ItemDataSO ItemData => data;


        // public InventoryController ContainIn { get; set; }

        private void Awake()
        {
            tag = "Interactable";
            
        }

        protected override void OnClientSpawn()
        {
            rb = GetComponent<Rigidbody>();
        }

        IEnumerator CheckItemStill()
        {
            yield return new WaitForSeconds(timeToCheckIfItemStill);
            //if(rb.angularVelocity.magnitude)
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