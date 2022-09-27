using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    public void Start()
    {
        pauseMenu = MenuManager.Instance.GetMenu(MenuType.Pause);
    }

    public void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape)
        //     && (MenuManager.Instance.GetCurrentMenu() == MenuType.Game
        //         || MenuManager.Instance.GetCurrentMenu() == MenuType.Pause))
        // {
        //     if (pauseMenu.IsOpen)
        //         MenuManager.Instance.CloseMenu(pauseMenu);
        //     else
        //         MenuManager.Instance.OpenMenu(pauseMenu);
        // }
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