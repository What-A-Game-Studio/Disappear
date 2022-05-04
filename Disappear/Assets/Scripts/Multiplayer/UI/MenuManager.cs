using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private MenuController[] menus;
    public static MenuManager Instance { get; set; }

    void Awake()
    {
        if (MenuManager.Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Activate menus base on name & close others
    /// </summary>
    /// <param name="menuName">Normally first char in uppercase</param>
    public void OpenMenu(string menuName)
    {
        foreach (MenuController menu in menus)
        {
            if (menu.MenuName == menuName)
            {
                OpenMenu(menu, false);
            }
            else
            {
                CloseMenu(menu);
            }
        }
    }

    /// <summary>
    /// Activate menu base on object
    /// </summary>
    /// <param name="menu">MenuController to activate</param>
    public void OpenMenu(MenuController menu, bool closeOthers = true)
    {
        if (closeOthers)
            foreach (MenuController menuToClose in menus)
                if (menuToClose != menu)
                    CloseMenu(menu);
        menu.Open();
    }

    /// <summary>
    /// Activate menu base on object
    /// </summary>
    /// <param name="menu">MenuController to activate</param>
    public void CloseMenu(MenuController menu)
    {
        menu.Close();
    }
}