using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class ItemSpawner : MonoBehaviour
{
    [field:SerializeField] public ItemType ItemType { get; protected set; }
    protected virtual void Awake(){}
    
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
    public abstract Vector3 SpawnCoordinate();    
    
}