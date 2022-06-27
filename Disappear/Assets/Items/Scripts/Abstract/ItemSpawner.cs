using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PhotonView))]
public abstract class ItemSpawner : MonoBehaviour
{
    [field:SerializeField] public ItemType ItemType { get; protected set; }
    private PhotonView pv;
    protected virtual void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    /// <summary>
    /// For the first call of this method we generate a random number [1:maxItemsToSpawn]
    /// who determinate how many items need to be instantiated
    /// </summary>
    /// <returns>Number of item to instantiate </returns>
    public abstract int GetNbItemToSpawn();
    
    /// <summary>
    /// Generate a Vector3 for item position
    /// </summary>
    /// <returns>Item position</returns>
    protected abstract Vector3 SpawnCoordinate();    
    
    public void InstantiateItem(int id)
    {
        Debug.Log(nameof(RPC_Instantiate));
        pv.RPC(nameof(RPC_Instantiate),RpcTarget.All,SpawnCoordinate(), id);
    }

    [PunRPC]
    protected virtual void RPC_Instantiate(Vector3 position, int itemToSpawn)
    {
        ItemDataSO item = ItemManager.Instance.GetItemById(itemToSpawn);
        if(item == null)
            return;
        
        GameObject go = Instantiate(item.Model, position, quaternion.identity);
        go.transform.parent = transform;
        go.layer = LayerMask.NameToLayer("Interactable");
        ItemController ic = go.AddComponent<ItemController>();
        ic.ItemData = item;
    }
}