using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        MenuManager.Instance.OpenMenu("Loading");
        //Use settings in Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings.asset
        Debug.Log("Connecting master server");
        PhotonNetwork.ConnectUsingSettings();
        
    }
    /// <summary>
    /// Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
    /// </summary>
    /// <remarks>
    /// The list of available rooms won't become available unless you join a lobby via LoadBalancingClient.OpJoinLobby.
    /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
    /// </remarks>
    public override void OnConnectedToMaster()
    {
        Debug.Log("Master Joined");
        Debug.Log("joining lobby");
        //Join a lobby
        PhotonNetwork.JoinLobby();
    }
    
    /// <summary>
    /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnRoomListUpdate.
    /// </summary>
    /// <remarks>
    /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify in the public cloud).
    /// The room list gets available via OnRoomListUpdate.
    /// </remarks>
    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("Title");
        Debug.Log("Lobby joined");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
