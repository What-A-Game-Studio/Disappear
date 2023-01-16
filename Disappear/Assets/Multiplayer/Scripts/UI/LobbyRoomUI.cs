using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Authentication;
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
        [SerializeField] private Transform seekerContainer;
        [SerializeField] private Transform hiderContainer;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button seekerJoinButton;
        [SerializeField] private Button hiderJoinButton;
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

            startGameButton.gameObject.SetActive(LobbyManager.Instance.IsHost);

            bool isEveryoneReady = true;
            List<Player> lobbyPlayers = LobbyManager.Instance.CurrentLobby.Players;
            foreach (Player player in lobbyPlayers)
            {
                PlayerListItemUI item = GetPlayerListItem(player);
                item.SetPlayerData(player);
                if (player.Data["Ready"].Value == "N")
                    isEveryoneReady = false;
            }

            startGameButton.interactable = isEveryoneReady && LobbyManager.Instance.IsHost;
        }

        private PlayerListItemUI GetPlayerListItem(Player player)
        {
            foreach (PlayerListItemUI displayedPlayer in displayedPlayers)
            {
                if (displayedPlayer.PlayerId == player.Id)
                    return displayedPlayer;
            }

            return CreatePlayerListItem(player.Data["Role"].Value);
        }

        private PlayerListItemUI CreatePlayerListItem(string role)
        {
            PlayerListItemUI playerItem =
                Instantiate(playerListItemPrefab.gameObject, role == "S" ? seekerContainer : hiderContainer)
                    .GetComponent<PlayerListItemUI>();
            playerItem.gameObject.SetActive(true);
            displayedPlayers.Add(playerItem);
            return playerItem;
        }

        public void JoinSeekerTeamOnClick()
        {
            PlayerListItemUI pli =
                displayedPlayers.FirstOrDefault(pli => pli.PlayerId == AuthenticationService.Instance.PlayerId);
            pli.transform.SetParent(seekerContainer);
            LobbyManager.Instance.UpdatePlayerDataInCurrentLobby(new Dictionary<string, PlayerDataObject>
            {
                {
                    "Role", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, value: "S")
                }
            });
        }

        public void JoinHiderTeamOnClick()
        {
            PlayerListItemUI pli =
                displayedPlayers.FirstOrDefault(pli => pli.PlayerId == AuthenticationService.Instance.PlayerId);
            pli.transform.SetParent(hiderContainer);
            LobbyManager.Instance.UpdatePlayerDataInCurrentLobby(new Dictionary<string, PlayerDataObject>
            {
                {
                    "Role", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, value: "H")
                }
            });
        }

        public void StartGameWithChosenScene()
        {
            NGOMultiplayerManager.Instance.LoadSceneByName(chosenScene, LoadSceneMode.Single);
        }
    }
}