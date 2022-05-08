using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolSystem : MonoBehaviour
{
    private static PoolSystem instance;
    public static PoolSystem Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<PoolSystem>();
            return instance;
        }
    }
    
    private Vector3 poolPosition;
    private Dictionary<string, GameObject> itemsChildren = new Dictionary<string, GameObject>();

    private void Awake()
    {
        instance = this;
        poolPosition = transform.position;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            itemsChildren.Add(child.name, child);
        }
    }

    public void SpawnItem(string itemName, Vector3 spawnPosition)
    {
        if (itemsChildren.ContainsKey(itemName))
            itemsChildren[itemName].transform.position = spawnPosition;
    }

    public void StoreInPool(string itemName)
    {
        if (itemsChildren.ContainsKey(itemName))
            itemsChildren[itemName].transform.position = poolPosition;
    }
}