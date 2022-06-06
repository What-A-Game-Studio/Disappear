using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnerManager : MonoBehaviour
{
    private static PlayerSpawnerManager instance;

    public static PlayerSpawnerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerSpawnerManager>();
            }
            return instance;
        }
    }

    private List<Vector3> playerSpawnPositionList = new List<Vector3>();


    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            playerSpawnPositionList.Add(transform.GetChild(i).position);
        }
    }

    public Vector3 SpawnPlayerToPosition()
    {
        
    }

}