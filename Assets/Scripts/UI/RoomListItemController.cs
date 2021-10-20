using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListItemController : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameTxt;

    [SerializeField] private TMP_Text nbPlayerTxt;

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