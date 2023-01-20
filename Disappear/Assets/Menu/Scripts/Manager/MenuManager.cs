using System;
using UnityEngine;
using UnityEngine.InputSystem;
using WAG.Core.Controls;
using WAG.Core.GM;

namespace WAG.Menu
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance { get; set; }
        [SerializeField] MenuController[] menus;
        [SerializeField] private MenuType defaultMenu;
        private MenuController currentMenu;

        private MenuType currentMenuType;

        private void Awake()
        {
            if (Instance != null)
                Destroy(this.gameObject);
            
            Instance = this;
            OpenMenu(defaultMenu);
            DontDestroyOnLoad(this.gameObject);
            
            InputManager.Instance.AddCallbackAction(ActionsControls.OpenMenu, OpenPauseMenu);
            InputManager.Instance.AddCallbackAction(ActionsControls.CloseMenu, ClosePauseMenu);
        }

        public MenuController GetMenu(MenuType menuName)
        {
            foreach (MenuController menu in menus)
                if (menu.MenuName == menuName)
                    return menu;

            throw new Exception("Missing menu");
        }

        public void OpenMenu(MenuType menuName)
        {
            foreach (MenuController menu in menus)
            {
                if (menu.MenuName != menuName && menu.IsOpen)
                    menu.SetMenuActiveState(false);
                else if (menu.MenuName == menuName && !menu.IsOpen)
                {
                    menu.SetMenuActiveState(true);
                    currentMenu = menu;
                }
            }
        }

        public void CloseMenu(MenuType menuName)
        {
            foreach (MenuController menu in menus)
                if (menu.MenuName == menuName)
                    menu.SetMenuActiveState(false);
        }

        public void OpenPreviousMenu()
        {
            OpenMenu(currentMenu.PreviousMenu);
        }
        
        
        public void OpenPauseMenu(InputAction.CallbackContext context)
        {
            OpenMenu(MenuType.Pause);
            GameManager.Instance.SwitchCursorLockMode(CursorLockMode.None, true);
        }

        public void ClosePauseMenu(InputAction.CallbackContext context)
        {
            CloseMenu(MenuType.Pause);
            GameManager.Instance.SwitchCursorLockMode(CursorLockMode.Locked, false);
            OpenMenu(MenuType.Game);
        }

        public void QuitGame()
        {
            GameManager.Instance.QuitGame();
        }
    }
}