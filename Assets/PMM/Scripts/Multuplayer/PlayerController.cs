using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master");
        }
        else
        {
            Debug.Log("Not master");
        }
    }
}
