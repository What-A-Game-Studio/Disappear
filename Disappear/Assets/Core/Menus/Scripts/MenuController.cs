using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WAG.Core.Menus
{

    public class MenuController : MonoBehaviour
    {
        [SerializeField] MenuType menuName;

        public MenuType MenuName
        {
            get { return menuName; }
        }

        public bool IsOpen;

        public void Open()
        {
            IsOpen = true;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            IsOpen = false;
            gameObject.SetActive(false);
        }
    }
}