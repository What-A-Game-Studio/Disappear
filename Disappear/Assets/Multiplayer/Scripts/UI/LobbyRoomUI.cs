using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace WAG.Multiplayer
{
    public class LobbyRoomUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private PlayerListItemUI playerListItemPrefab;
        private List<PlayerListItemUI> displayedPlayers = new List<PlayerListItemUI>();
        private float refreshLobbyRoomTimer;
        [SerializeField] private float refreshLobbyRoomTimerTimerMax;

        private void Awake()
        {
            playerListItemPrefab.gameObject.SetActive(false);
            refreshLobbyRoomTimer = refreshLobbyRoomTimerTimerMax;
        }

        private void Update()
        {
            HandleLobbyRoomUIUpdate();
        }

        private void HandleLobbyRoomUIUpdate()
        {
            if (LobbyManager.Instance.CurrentLobby == null) return;
            refreshLobbyRoomTimer -= Time.deltaTime;
            if (!(refreshLobbyRoomTimer < 0)) return;
            refreshLobbyRoomTimer = refreshLobbyRoomTimerTimerMax;
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (lobbyNameText.text != LobbyManager.Instance.CurrentLobby.Name)
            {
                lobbyNameText.text = LobbyManager.Instance.CurrentLobby.Name;
            }

            List<Player> lobbyPlayers = LobbyManager.Instance.CurrentLobby.Players;
            foreach (Player player in lobbyPlayers)
            {
                PlayerListItemUI item = GetPlayerListItem(player);
                item.SetPlayerData(player);
            }
        }

        private PlayerListItemUI GetPlayerListItem(Player player)
        {
            foreach (PlayerListItemUI displayedPlayer in displayedPlayers)
            {
                if (displayedPlayer.PlayerId == player.Id)
                {
                    return displayedPlayer;
                }
            }

            return CreatePlayerListItem();
        }

        private PlayerListItemUI CreatePlayerListItem()
        {
            PlayerListItemUI playerItem =
                Instantiate(playerListItemPrefab.gameObject, playerListItemPrefab.transform.parent)
                    .GetComponent<PlayerListItemUI>();
            playerItem.gameObject.SetActive(true);
            displayedPlayers.Add(playerItem);
            return playerItem;
        }
    }
}