using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class DebuggerScript : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
           // PoolSystem.Instance.SpawnItem("Cube", new Vector3(0.0f, 1.3f, 1.8f));
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
           // PoolSystem.Instance.StoreInPool("Cube");

        }
    }
}
