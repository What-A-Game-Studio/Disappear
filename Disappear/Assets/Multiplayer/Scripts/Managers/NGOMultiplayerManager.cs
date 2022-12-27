using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using WAG.Menu;

namespace WAG.Multiplayer
{
    public class NGOMultiplayerManager : MonoBehaviour
    {
        public static NGOMultiplayerManager Instance { get; private set; }


        [SerializeField] private FindLobbyUI findLobbyMenu;
        [SerializeField] private LobbyRoomUI lobbyRoomMenu;
        [SerializeField] private TMP_InputField playerNameInputField;
        private LocalPlayerData localPlayer;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }

            Instance = this;
        }

        /// <summary>
        /// Try to anonymously sign in the player at launch
        /// </summary>
        public void TrySignIn()
        {
            string name;
            if (string.IsNullOrEmpty(playerNameInputField.text))
                name = "Random Player";
            else
                name = playerNameInputField.text;

            AuthenticationAPIInterface.InitializeAndSignInAsync(name);
            localPlayer = new LocalPlayerData(name);
            MenuManager.Instance.OpenMenu(MenuType.MainMenu);
        }

        /// <summary>
        /// Open the Lobby Creation menu
        /// </summary>
        public void NewLobbyOnClick()
        {
            MenuManager.Instance.OpenMenu(MenuType.CreateLobby);
        }

        /// <summary>
        /// Open the Lobby List menu
        /// </summary>
        public void SearchLobbyOnClick()
        {
            MenuManager.Instance.OpenMenu(MenuType.FindLobby);
            UpdateLobbyListUI();
        }


        /// <summary>
        /// Update the the lobby list interface
        /// </summary>
        public void UpdateLobbyListUI()
        {
            findLobbyMenu.UpdateUI();
        }


        /// <summary>
        /// Update the the lobby room interface
        /// </summary>
        public void UpdateLobbyRoomUI()
        {
            lobbyRoomMenu.UpdateUI();
        }


        /// <summary>
        /// Remove the local player from the lobby
        /// </summary>
        public void QuitLobbyOnClick()
        {
            LobbyAPIInterface.TryLeaveLobby(LobbyManager.Instance.CurrentLobby.Id);
            LobbyManager.Instance.LeaveLobby();

            MenuManager.Instance.OpenMenu(MenuType.MainMenu);
        }

        /// <summary>
        /// Create a new Lobby
        /// </summary>
        /// <param name="lobbyName"> the name of the lobby</param>
        /// <param name="maxPlayers"> the max number of players that can join the lobby </param>
        /// <param name="isPrivate"> the private state of the lobby</param>
        public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
        {
            string relayCode = await RelayAPIInterface.CreateRelay(maxPlayers);
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Player = localPlayer.GetLocalPlayerData(),
                Data = new Dictionary<string, DataObject>()
                {
                    {
                        "RelayCode", new DataObject(DataObject.VisibilityOptions.Member, value: relayCode)
                    }
                }
            };
            Lobby createdLobby = await LobbyAPIInterface.TryCreateLobby(lobbyName,
                maxPlayers,
                lobbyOptions);
            LobbyManager.Instance.StartLobbyLogic(createdLobby);
            UpdateLobbyRoomUI();
        }

        /// <summary>
        /// Make the local player join a lobby by id
        /// </summary>
        /// <param name="lobbyId"> The ID of the lobby to join</param>
        public async void JoinLobby(string lobbyId)
        {
            LobbyManager.Instance.StartLobbyLogic(
                await LobbyAPIInterface.TryJoinLobbyById(lobbyId, localPlayer.GetLocalPlayerData()));
            UpdateLobbyRoomUI();
            RelayAPIInterface.JoinRelay(LobbyManager.Instance.CurrentLobby.Data["RelayCode"].Value);
        }

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}