using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace WAG.Multiplayer
{
    public class FindLobbyUI : MonoBehaviour
    {
        [SerializeField] private LobbyListItemUI lobbyListItemPrefab;
        private List<LobbyListItemUI> lobbiesAlreadyDisplayed = new List<LobbyListItemUI>();
        private float refreshLobbyListTimer;
        [SerializeField] private float refreshLobbyListTimerMax;

        private void Awake()
        {
            lobbyListItemPrefab.gameObject.SetActive(false);
            refreshLobbyListTimer = refreshLobbyListTimerMax;
        }

        private void Update()
        {
            HandleLobbyListUIUpdate();
        }

        private void HandleLobbyListUIUpdate()
        {
            refreshLobbyListTimer -= Time.deltaTime;
            if (!(refreshLobbyListTimer < 0)) return;
            refreshLobbyListTimer = refreshLobbyListTimerMax;
            UpdateUI();
        }

        public async void UpdateUI()
        {
            QueryResponse lobbies = await LobbyAPIInterface.TryQueryLobbies();
            foreach (Lobby lobby in lobbies.Results)
            {
                LobbyListItemUI item = GetLobbyListItem(lobby);
                item.SetLobbyData(lobby);
            }
        }

        private LobbyListItemUI GetLobbyListItem(Lobby lobbiesResult)
        {
            foreach (LobbyListItemUI openedLobby in lobbiesAlreadyDisplayed)
            {
                if (lobbiesResult.Id == openedLobby.LobbyId)
                {
                    return openedLobby;
                }
            }

            return CreateLobbyListItem();
        }

        private LobbyListItemUI CreateLobbyListItem()
        {
            LobbyListItemUI listItem =
                Instantiate(lobbyListItemPrefab.gameObject, lobbyListItemPrefab.transform.parent)
                    .GetComponent<LobbyListItemUI>();
            listItem.gameObject.SetActive(true);
            lobbiesAlreadyDisplayed.Add(listItem);
            return listItem;
        }
    }
}