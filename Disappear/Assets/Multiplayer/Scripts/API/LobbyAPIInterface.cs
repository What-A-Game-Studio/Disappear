using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace WAG.Multiplayer
{
    /// <summary>
    /// Wrapper for all the interactions with the Lobby API.
    /// </summary>
    public static class LobbyAPIInterface
    {
        /// <summary>
        /// Create a new lobby
        /// </summary>
        /// <param name="requesterId"> Id of the player requesting the creation of the lobby </param>
        /// <param name="lobbyName"> Name of the lobby </param>
        /// <param name="maxPlayers"> The limit of players in the player </param>
        /// <param name="isPrivate"> Set the lobby privacy option </param>
        /// <returns> The created lobby</returns>
        public static async Task<Lobby> TryCreateLobby(string lobbyName, int maxPlayers, CreateLobbyOptions options)
        {
            //if the player didn't write a name, set a default name instead
            if (string.IsNullOrEmpty(lobbyName))
            {
                lobbyName = "New Lobby";
            }

            try
            {
                var lobbyCreated = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
                Debug.Log($"Lobby {lobbyCreated.Name}, {lobbyCreated.IsPrivate}");
                return lobbyCreated;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        public static async Task<Lobby> TryUpdateLobby(string lobbyId, Dictionary<string, DataObject> updatedData)
        {
            try
            {
                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(lobbyId, new UpdateLobbyOptions
                {
                    Data = updatedData
                });
                return lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log("e");
                throw;
            }
        }


        public static async Task<Lobby> TryUpdatePlayerInLobby(string lobbyId,
            Dictionary<string, PlayerDataObject> updatedPlayerData)
        {
            try
            {
                string playerId = AuthenticationService.Instance.PlayerId;
                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(lobbyId, playerId, new UpdatePlayerOptions
                {
                    Data = updatedPlayerData
                });
                return lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                throw;
            }
        }

        /// <summary>
        /// Find the active lobbies
        /// </summary>
        /// <returns> The found lobbies correspind to the query (actually no query specified so it returns every active lobbies) </returns>
        public static async Task<QueryResponse> TryQueryLobbies()
        {
            try
            {
                return await Lobbies.Instance.QueryLobbiesAsync();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                throw;
            }
        }

        public static async Task<Lobby> TryJoinLobbyById(string lobbyId, Player player)
        {
            try
            {
                return await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, new JoinLobbyByIdOptions
                {
                    Player = player
                });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                throw;
            }
        }

        public static async Task<Lobby> TryJoinLobbyByCode(string lobbyCode)
        {
            try
            {
                return await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                throw;
            }
        }

        public static async void TryLeaveLobby(string lobbyId)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        public static async void DeleteLobby(string lobbyId)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}