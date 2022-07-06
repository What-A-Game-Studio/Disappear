using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerListItemController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    TMP_Text playerNameTxt;

    [SerializeField]
    TMP_Text PlayerPingTxt;

    [SerializeField]
    Image playerHostImage;

    [Header("Teams")]
    [SerializeField]
    Image playerTeamBtn;

    [SerializeField] private Sprite seekerSprite;
    [SerializeField] private Sprite hiderSprite;
    private MultiplayerManager MultiplayerManager;
    public Player playerInfo { get; private set; }
    public bool isSeeker { get; private set; } = false;
    public void Init(Player pi, MultiplayerManager mm)
    {
        this.playerInfo = pi;
        playerNameTxt.text = pi.NickName;
        playerHostImage.gameObject.SetActive(pi.IsMasterClient);
        // SetTeam( pi.IsMasterClient, false);
        this.MultiplayerManager = mm;

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (Equals(playerInfo, otherPlayer))
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

    public void SetTeam(bool isS, bool notify = true)
    {
        isSeeker = isS;
        
        Hashtable customProperties = playerInfo.CustomProperties;
        if (isSeeker)
        {
            playerTeamBtn.sprite = seekerSprite;
            customProperties["team"] = true;
        }
        else
        {
            customProperties["team"] =false;
            playerTeamBtn.sprite = hiderSprite;
        }
        
        if(notify)
            PhotonNetwork.SetPlayerCustomProperties(customProperties);
        
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer.NickName != playerInfo.NickName) 
            return;
        
        if ((bool)changedProps["team"])
        {
            playerTeamBtn.sprite = seekerSprite;
        }
        else
        {
            playerTeamBtn.sprite = hiderSprite;
        }
    }

    public void ChangeTeam()
    {
        if(!PhotonNetwork.IsMasterClient)
            return;
        // MultiplayerManager.SetTeam();
        SetTeam(!isSeeker);
    }
    
}