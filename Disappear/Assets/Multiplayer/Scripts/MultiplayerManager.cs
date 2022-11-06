using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WaG;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    public static string PhotonPrefabPath { get; private set; } = "PhotonPrefabs";
    public static MultiplayerManager Instance { get; set; }

    [field: Header("Multiplayer Base Parameters")]
    [field: SerializeField]
    public int MaxSeekers { get; private set; }

    [field: SerializeField] public int MaxHiders { get; private set; }
    private int maxPlayers;

    [Space(10)] [SerializeField] private MenuType defaultMenu = MenuType.Loading;

    [Header("Inputs")] [SerializeField] TMP_InputField roomNameInput;

    [Header("Text")] [SerializeField] TMP_Text titleText;

    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;

    [Header("Containers / Entities")] [SerializeField]
    Transform roomListContent;

    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] RectTransform seekerListContent;
    [SerializeField] RectTransform hiderListContent;
    [SerializeField] GameObject playerListItemPrefab;

    [Header("Button")] [SerializeField] GameObject StartGameBtn;
    [SerializeField] private JoinTeamButton seekerJoinButton;
    [SerializeField] private JoinTeamButton hiderJoinButton;

    [Header("DEBUGGER A SUPPRIMER A LA FIN")] [SerializeField]
    private DebuggerManager dm;

    List<RoomInfo> roomList = new List<RoomInfo>();
    private List<string> playersDiplayed = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        maxPlayers = MaxHiders + MaxSeekers;
        dm.Init();
    }

    void Start()
    {
        if (defaultMenu != MenuType.None)
            MenuManager.Instance.OpenMenu(defaultMenu);

        PhotonNetwork.ConnectUsingSettings();
    }

    #region ======================= Public : Start  =======================

    public void CreateRoom()
    {
        Hashtable customProperties = new Hashtable();

        string roomName = "Room#";
        if (!string.IsNullOrEmpty(roomNameInput.text))
            roomName = roomNameInput.text;
        else
            roomName += Random.Range(0, 10000).ToString("0000");

        customProperties.Add("CH", 0);
        customProperties.Add("CS", 0);
        customProperties.Add("MH", MaxHiders);
        customProperties.Add("MS", MaxSeekers);

        PhotonNetwork.CreateRoom(roomName,
            new RoomOptions { MaxPlayers = (byte)maxPlayers, CustomRoomProperties = customProperties, BroadcastPropsChangeToAll = true });
        MenuManager.Instance.OpenMenu(MenuType.Loading);
    }

    public void LeaveRoom()
    {
        LeaveRoom(QuitEnum.Quit);
    }

    public void LeaveRoom(QuitEnum reasonToQuit = QuitEnum.Quit)
    {
        object[] content = new object[] { PhotonNetwork.NickName, reasonToQuit };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(1, content, raiseEventOptions, SendOptions.SendReliable);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MultiplayerMenuScene");
        // PhotonNetwork.LeaveRoom();
        // MenuManager.Instance.OpenMenu(MenuType.Loading);
        // SceneManager.LoadScene(0);
    }

    public void JoinHiderTeam()
    {
        MenuManager.Instance.OpenMenu(MenuType.Room);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        SetTeam("Hider");
        if (!PhotonNetwork.IsMasterClient)
            FillPlayerRoomList();
        StartGameBtn.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void JoinSeekerTeam()
    {
        MenuManager.Instance.OpenMenu(MenuType.Room);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        SetTeam("Seeker");
        if (!PhotonNetwork.IsMasterClient)
            FillPlayerRoomList();

        StartGameBtn.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void ChangeTeamToSeeker()
    {
        SetTeam("Seeker");
    }

    public void ChangeTeamToHider()
    {
        SetTeam("Hider");
    }

    public void SetTeam(string team)
    {
        Hashtable customProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        customProperties["team"] = team;
        PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
    }


    public void JoinRoom(RoomInfo info)
    {
        if (info.MaxPlayers > info.PlayerCount)
        {
            PhotonNetwork.JoinRoom(info.Name);
            MenuManager.Instance.OpenMenu(MenuType.Loading);
        }
        else
        {
            errorText.text = "ROOM IS FULL";
            MenuManager.Instance.OpenMenu(MenuType.Error);
        }
    }

    public void JoinRoom()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName))
            return;

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].Name == roomName)
            {
                if (roomList[i].MaxPlayers <= roomList[i].PlayerCount)
                {
                    errorText.text = "ROOM IS FULL";
                    MenuManager.Instance.OpenMenu(MenuType.Error);
                    return;
                }
            }
            else
            {
                errorText.text = "The room does not exist";
                MenuManager.Instance.OpenMenu(MenuType.Error);
                return;
            }
        }

        PhotonNetwork.JoinRoom(roomName);
        MenuManager.Instance.OpenMenu(MenuType.Loading);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void SetGameTitle(string name)
    {
        titleText.text = name;
    }

    #endregion ======================= Public : end  =======================

    #region ======================= Private : Start  =======================

    private void CreatePlayer(Player player)
    {
        if (playersDiplayed.Contains(player.NickName))
            return;

        GameObject playerGo;
        if ((string)player.CustomProperties["team"] == "Seeker")
        {
            playerGo = Instantiate(playerListItemPrefab, seekerListContent);
            UpdateRoomTeamData("CS", 1);
        }
        else
        {
            playerGo = Instantiate(playerListItemPrefab, hiderListContent);
            UpdateRoomTeamData("CH", 1);
        }

        playerGo.name = player.NickName;
        playersDiplayed.Add(player.NickName);
        playerGo.GetComponent<PlayerListItemController>().Init(player);
    }

    private void ClearPlayerRoomList()
    {
        foreach (Transform child in seekerListContent)
        {
            Destroy(child.gameObject);
        }
    }

    private void FillPlayerRoomList()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
            CreatePlayer(players[i]);
    }

    private void UpdateRoomTeamData(string team, int modifier)
    {
        int newCount = (int)PhotonNetwork.CurrentRoom.CustomProperties[team];
        newCount += modifier;
        PhotonNetwork.CurrentRoom.CustomProperties[team] = newCount;
    }

    #endregion ======================= Private : End  =======================

    #region ======================= Photon Override : Start  =======================

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu(MenuType.Title);
        //En attendant la saisie du joueur
        PhotonNetwork.NickName = "Player#" + Random.Range(0, 10000).ToString("0000");
    }

    public override void OnCreatedRoom()
    {
        hiderJoinButton.Init((int)PhotonNetwork.CurrentRoom.CustomProperties["CH"], (int)PhotonNetwork.CurrentRoom.CustomProperties["MH"]);
        seekerJoinButton.Init((int)PhotonNetwork.CurrentRoom.CustomProperties["CS"], (int)PhotonNetwork.CurrentRoom.CustomProperties["MS"]);
    }


    public override void OnJoinedRoom()
    {
        // hiderJoinButton.Init((int)PhotonNetwork.CurrentRoom.CustomProperties["CurrentHiders"], (int)PhotonNetwork.CurrentRoom.CustomProperties["MaxHiders"]);
        // seekerJoinButton.Init((int)PhotonNetwork.CurrentRoom.CustomProperties["CurrentSeekers"], (int)PhotonNetwork.CurrentRoom.CustomProperties["MaxHiders"]);
        MenuManager.Instance.OpenMenu(MenuType.Role);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        Debug.LogError("Room Creation Failed: " + message, this);
        MenuManager.Instance.OpenMenu(MenuType.Error);
    }


    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu(MenuType.Title);
        ClearPlayerRoomList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //CreatePlayer(newPlayer);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        this.roomList = roomList;
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            if (roomList[i].MaxPlayers > roomList[i].PlayerCount)
                Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItemController>()
                    .Init(roomList[i]);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // ClearPlayerRoomList();
        // FillPlayerRoomList();
        //
        // StartGameBtn.SetActive(PhotonNetwork.IsMasterClient);
        // rulesBtn.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnPlayerPropertiesUpdate(Player target, Hashtable changeProps)
    {
        CreatePlayer(target);
    }

    public override void OnRoomPropertiesUpdate(Hashtable changeProps)
    {
    }

    #endregion ======================= Photon Override : End  =======================
}