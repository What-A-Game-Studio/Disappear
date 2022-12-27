using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace WAG.Multiplayer
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }
        public Lobby CurrentLobby { get; private set; }
        private bool isHost;
        private float heartbeatTimer;
        [SerializeField] private float heartbeatTimerMax;
        [SerializeField] private float lobbyUpdateTimerMax;
        private float lobbyUpdateTimer;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
            }

            Instance = this;

            heartbeatTimer = heartbeatTimerMax;
            lobbyUpdateTimer = lobbyUpdateTimerMax;
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
            HandleLobbyPollForUpdate();
        }


        public void StartLobbyLogic(Lobby lobby)
        {
            CurrentLobby = lobby;
            isHost = lobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        public void LeaveLobby()
        {
            CurrentLobby = null;
            isHost = false;
            Debug.Log("Left Lobby");
        }

        /// <summary>
        /// Send a heartbeat to the current lobby at a regular time to keep it active
        /// </summary>
        private async void HandleLobbyHeartbeat()
        {
            if (!isHost || CurrentLobby == null) return;
            heartbeatTimer -= Time.deltaTime;
            if (!(heartbeatTimer < 0)) return;
            heartbeatTimer = heartbeatTimerMax;
            await LobbyService.Instance.SendHeartbeatPingAsync(CurrentLobby.Id);
        }

        /// <summary>
        /// Lobby only update information for host.
        /// We need to call this function to update it on player side
        /// </summary>
        private async void HandleLobbyPollForUpdate()
        {
            if (CurrentLobby == null) return;
            lobbyUpdateTimer -= Time.deltaTime;
            if (!(lobbyUpdateTimer < 0)) return;
            lobbyUpdateTimer = lobbyUpdateTimerMax;
            CurrentLobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);
        }

        /// <summary>
        /// Update the data of the current lobby
        /// </summary>
        /// <param name="updates"> new data to save in the lobby</param>
        public async void UpdateCurrentLobby(Dictionary<string, DataObject> updates)
        {
            await LobbyAPIInterface.TryUpdateLobby(CurrentLobby.Id, updates);
        }

        /// <summary>
        /// Update the data of the local player in the current lobby
        /// </summary>
        /// <param name="updates"> new data to save in the lobby </param>
        public async void UpdatePlayerDataInCurrentLobby(Dictionary<string, PlayerDataObject> updates)
        {
           await LobbyAPIInterface.TryUpdatePlayerInLobby(CurrentLobby.Id, updates);
        }
    }
}