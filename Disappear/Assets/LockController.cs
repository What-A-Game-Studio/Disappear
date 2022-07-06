using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockController : MonoBehaviour
{
 
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        PlayerController.MainPlayer.enabled = false;
        PlayerController.MainPlayer.CameraController.CanRotate = false;
    }

    private void OnDisable()
    {
        if (PlayerController.MainPlayer)
        {
            Debug.Log("Locked by ", this);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerController.MainPlayer.enabled = true;
            PlayerController.MainPlayer.CameraController.CanRotate = true;
        }
    }
}