using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomListItemController : MonoBehaviour
{

    [SerializeField]
    TMP_Text roomNameTxt;

    [SerializeField]
    TMP_Text nbPlayerTxt;

    private RoomInfo roomInfo;

    public void Init(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        roomNameTxt.text = roomInfo.Name;
        nbPlayerTxt.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;

    }

    public void JoinRoom()
    {
        MultiplayerManager.Instance.JoinRoom(roomInfo);
    }
}
