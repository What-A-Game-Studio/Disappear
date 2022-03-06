using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItemController : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text playerNameTxt;

    [SerializeField] private TMP_Text PlayerPingTxt;

    [SerializeField] private Image playerHostImage;

    private Player playerInfo;

    public void Init(Player playerInfo)
    {
        this.playerInfo = playerInfo;
        playerNameTxt.text = playerInfo.NickName;
        playerHostImage.gameObject.SetActive(playerInfo.IsMasterClient);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playerInfo == otherPlayer) Destroy(gameObject);
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}