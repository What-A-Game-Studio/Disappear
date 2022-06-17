using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoomManager : MonoBehaviourPunCallbacks, IPunObservable, IOnEventCallback
{
    #region Rules parameters
    public bool separateControls { get; private set; }
    #endregion

    private static RoomManager instance;
    public static RoomManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<RoomManager>();
            return instance;
        }
        set { instance = value; }
    }
    private const byte playerQuitEventCode = 1;
    private string leavingPlayer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    #region  ======================= Public : Start  =======================
    public void SetSeparationControlsState(bool state)
    {
        separateControls = state;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(separateControls);
        }
        else
        {
            separateControls = (bool)stream.ReceiveNext();
        }
    }

    public void OnPlayerLeave()
    {
        object[] content = new object[] { PhotonNetwork.NickName }; 
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(playerQuitEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MultiplayerMenuScene");
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == playerQuitEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            leavingPlayer = (string)data[0];
            if(leavingPlayer != PhotonNetwork.NickName)
            {
                MenuManager.Instance.GetMenu(MenuType.Victory).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = leavingPlayer + " has left the game.";
                MenuManager.Instance.OpenMenu(MenuType.Victory);
            }
        }
    }


    #endregion ======================= Public : Start  =======================

    #region  ======================= Private : Start  =======================
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1) //if game secene 
        {
            //Instantiate RoomMangar at (0,0,0) because it's a Empty
            PhotonNetwork.Instantiate(Path.Combine(MultiplayerManager.PhotonPrefabPath, "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }   
    #endregion ======================= Private : Start  =======================
}
