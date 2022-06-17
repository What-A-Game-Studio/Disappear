using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; set; }
    [SerializeField]
    MenuController[] menus;

    private MenuType currentMenuType;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public MenuController GetMenu(MenuType menuName)
    {
        for (int i = 0; i < menus.Length; i++)
            if (menus[i].MenuName == menuName)
                return menus[i];
        throw new Exception("Missing menu");
    }

    public void OpenMenu(MenuType menuName)
    {
        for (int i = 0; i < menus.Length; i++)
            if (menus[i].MenuName == menuName)
                OpenMenu(menus[i]);
    }
    public void OpenMenu(MenuController menu)
    {
        for (int i = 0; i < menus.Length; i++)
            if (menus[i].IsOpen)
                CloseMenu(menus[i]);
        menu.Open();
        currentMenuType = menu.MenuName;
    }
    public void CloseMenu(MenuController menu)
    {
        menu.Close();
    }

    public void CloseAll()
    {
        for (int i = 0; i < menus.Length; i++)
            if (menus[i].IsOpen)
                CloseMenu(menus[i]);
    }

    public MenuType GetCurrentMenu()
    {
        return currentMenuType;
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}
