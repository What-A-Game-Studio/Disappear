using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WAG.Multiplayer
{
    public class LobbyRoomUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private PlayerListItemUI playerListItemPrefab;
        [SerializeField] private Button startGameButton;
        [SerializeField] private string chosenScene;
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

            if (LobbyManager.Instance.IsHost)
            {
                startGameButton.gameObject.SetActive(true);
            }
            else
            {
                startGameButton.gameObject.SetActive(false);
            }

            bool isEveryoneReady = true;
            List<Player> lobbyPlayers = LobbyManager.Instance.CurrentLobby.Players;
            foreach (Player player in lobbyPlayers)
            {
                PlayerListItemUI item = GetPlayerListItem(player);
                item.SetPlayerData(player);
                if (player.Data["Ready"].Value == "N")
                    isEveryoneReady = false;
            }

            startGameButton.interactable = isEveryoneReady;
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

        public void StartGameWithChosenScene()
        {
            NGOMultiplayerManager.Instance.LoadSceneByName(chosenScene, LoadSceneMode.Single);
        }
    }
}