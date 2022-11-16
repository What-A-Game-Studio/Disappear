using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace WAG.Multiplayer
{
    public class PlayerListItemController : MonoBehaviourPunCallbacks
    {
        [SerializeField] TMP_Text playerNameTxt;

        [SerializeField] TMP_Text PlayerPingTxt;

        [SerializeField] Image playerHostImage;

        public Player playerInfo { get; private set; }

        public void Init(Player pi)
        {
            this.playerInfo = pi;
            playerNameTxt.text = pi.NickName;
            if (pi.NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                playerNameTxt.color = Color.yellow;
            }

            playerHostImage.gameObject.SetActive(pi.IsMasterClient);
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

        public void SetTeam(string team)
        {
            Hashtable customProperties = playerInfo.CustomProperties;
            customProperties["team"] = team;
            playerInfo.SetCustomProperties(customProperties);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (targetPlayer.NickName != playerInfo.NickName)
                return;

            // if ((bool)changedProps["team"])
            // {
            //     playerTeamBtn.sprite = seekerSprite;
            // }
            // else
            // {
            //     playerTeamBtn.sprite = hiderSprite;
            // }
        }

        public void SetTeamToHider()
        {
            SetTeam("Hider");
        }

        public void SetTeamToSeeker()
        {
            SetTeam("Hider");
        }
    }
}