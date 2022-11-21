using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAG.Core.Controls;
using WAG.Player;

namespace WAG.Items
{
    public class TrapUIController : MonoBehaviour
    {
        public PlayerController PlayerController { get; set; }

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            InputManager.Instance.SwitchMap(ControlMap.Menu);
        }

        public void SetItFree()
        {
            InputManager.Instance.SwitchMap(ControlMap.Player);
            PlayerController.CanMove = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Destroy(gameObject);
        }
    }
}
