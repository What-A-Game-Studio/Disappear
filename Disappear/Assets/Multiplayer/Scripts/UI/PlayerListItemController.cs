using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItemController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_Text playerNameTxt;

    [SerializeField]
    TMP_Text PlayerPingTxt;

    [SerializeField]
    Image playerHostImage;

    private Player playerInfo;

    public void Init(Player playerInfo)
    {
        this.playerInfo = playerInfo;
        playerNameTxt.text = playerInfo.NickName;
        playerHostImage.gameObject.SetActive(playerInfo.IsMasterClient);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playerInfo == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
