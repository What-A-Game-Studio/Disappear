using ExitGames.Client.Photon.StructWrapping;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using WaG.Input_System.Scripts;

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
        InputManager.Instance.AddCallbackAction(ActionsControls.OpenMenu, OpenPauseMenu);
        InputManager.Instance.AddCallbackAction(ActionsControls.CloseMenu, ClosePauseMenu);
        
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

    public void QuitRoom()
    {
        RoomManager.Instance.OnPlayerLeave();
        MenuManager.Instance.OpenMenu(MenuType.Loading);
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

    public void HiderQuit(QuitEnum reasonToQuit)
    {
        MultiplayerManager.Instance.LeaveRoom(reasonToQuit);
    }

    public void SwitchCursorLockMode(CursorLockMode lockMode, bool visible)
    
    {
        Cursor.lockState = lockMode;
        Cursor.visible = visible;
        if (PlayerController.MainPlayer)
        {
            PlayerController.MainPlayer.CanMoveOrRotate = !visible;

        }
    }
}