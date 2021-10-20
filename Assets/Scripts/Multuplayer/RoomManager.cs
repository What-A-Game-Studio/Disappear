using System.IO;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks, IPunObservable, IOnEventCallback
{
    private const byte playerQuitEventCode = 1;
    private string leavingPlayer;

    #region Rules parameters

    public bool separateControls { get; private set; }

    #endregion

    public static RoomManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Instance = this;
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

    #region ======================= Private : Start  =======================

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "MultiplayerGameScene") //if game secene 
            //Instantiate RoomMangar at (0,0,0) because it's a Empty
            PhotonNetwork.Instantiate(Path.Combine(MultiplayerManager.PhotonPrefabPath, "PlayerManager"), Vector3.zero,
                Quaternion.identity);
    }

    #endregion ======================= Private : Start  =======================

    #region ======================= Public : Start  =======================

    public void SetSeparationControlsState(bool state)
    {
        separateControls = state;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(separateControls);
        else
            separateControls = (bool) stream.ReceiveNext();
    }

    public void OnPlayerLeave()
    {
        object[] content = {PhotonNetwork.NickName};
        var raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent(playerQuitEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MultiplayerMenuScene");
    }

    public void OnEvent(EventData photonEvent)
    {
        var eventCode = photonEvent.Code;
        if (eventCode == playerQuitEventCode)
        {
            var data = (object[]) photonEvent.CustomData;
            leavingPlayer = (string) data[0];
            if (leavingPlayer != PhotonNetwork.NickName)
            {
                MenuManager.Instance.GetMenu(MenuType.Victory).transform.GetChild(0).GetComponent<TextMeshProUGUI>()
                    .text = leavingPlayer + " has left the game.";
                MenuManager.Instance.OpenMenu(MenuType.Victory);
            }
        }
    }

    #endregion ======================= Public : Start  =======================
}