using ExitGames.Client.Photon.StructWrapping;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using WAG.Core.Controls;


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

        
        InputManager.Instance.AddCallbackAction(ActionsControls.OpenMenu, OpenPauseMenu);
        InputManager.Instance.AddCallbackAction(ActionsControls.CloseMenu, ClosePauseMenu);
        
    }

    public void OpenPauseMenu(InputAction.CallbackContext context)
    {
        if (MenuManager.Instance.GetCurrentMenu() == MenuType.Game)
            MenuManager.Instance.OpenMenu(pauseMenu);
    }

    public void ClosePauseMenu(InputAction.CallbackContext context)
    {
        MenuManager.Instance.CloseMenu(pauseMenu);
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
}