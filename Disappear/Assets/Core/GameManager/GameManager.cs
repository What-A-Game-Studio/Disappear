using UnityEngine;
using UnityEngine.InputSystem;
using WAG.Core.Controls;
using WAG.Core.Menus;

namespace WAG.Core.GM
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private MenuController pauseMenu;

        private void Awake()
        {
            //Singleton pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(gameObject);

            SwitchCursorLockMode(CursorLockMode.None, true);
            WAG.Core.Controls.InputManager.Instance.AddCallbackAction(ActionsControls.OpenMenu, OpenPauseMenu);
            WAG.Core.Controls.InputManager.Instance.AddCallbackAction(ActionsControls.CloseMenu, ClosePauseMenu);
        }

        public void OpenPauseMenu(InputAction.CallbackContext context)
        {
            MenuManager.Instance.OpenMenu(pauseMenu);
            SwitchCursorLockMode(CursorLockMode.None, true);
        }

        public void ClosePauseMenu(InputAction.CallbackContext context)
        {
            MenuManager.Instance.CloseMenu(pauseMenu);
            SwitchCursorLockMode(CursorLockMode.Locked, false);
            MenuManager.Instance.OpenMenu(MenuType.Game);
        }

        public void Start()
        {
            pauseMenu = MenuManager.Instance.GetMenu(MenuType.Pause);
        }

        public void QuitGame()
        {
#if UNITY_WEBPLAYER
     public static string webplayerQuitURL = "http://ronan-dhersignerie.fr/";
#elif UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
        }
        

        public void SwitchCursorLockMode(CursorLockMode lockMode, bool visible)

        {
            Cursor.lockState = lockMode;
            Cursor.visible = visible;
        }
    }
}