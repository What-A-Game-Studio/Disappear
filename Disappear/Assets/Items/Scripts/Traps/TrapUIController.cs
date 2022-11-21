using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAG.Player;

namespace WAG.Items
{
    public class TrapUIController : MonoBehaviour
    {
        public PlayerController PlayerController { get; set; }

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void SetItFree()
        {
            PlayerController.CanMove = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Destroy(gameObject);
        }
    }
}
