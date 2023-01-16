using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using WAG.Core.GM;
using WAG.Menu;

namespace WAG.Multiplayer
{
    public class LobbyListItemUI : MonoBehaviour
    {
        private TextMeshProUGUI lobbyNameText;
        private TextMeshProUGUI playerCountText;
        public string LobbyId { get; private set; }

        private void Awake()
        {
            if (!transform.Find("LobbyName").TryGetComponent(out lobbyNameText))
            {
                Debug.LogError($"Missing {lobbyNameText.GetType()} component on {this.name}");
            }

            if (!transform.Find("PlayerCount").TryGetComponent(out playerCountText))
            {
                Debug.LogError($"Missing {playerCountText.GetType()} component on {this.name}");
            }
        }

        public void SetLobbyData(Lobby lobbyData)
        {
            lobbyNameText.SetText(lobbyData.Name);
            playerCountText.SetText($"{lobbyData.Players.Count} / {lobbyData.MaxPlayers}");
            LobbyId = lobbyData.Id;
        }   

        public async void JoinLobbyOnClick()
        {
            await NGOMultiplayerManager.Instance.JoinLobby(LobbyId);
            MenuManager.Instance.OpenMenu(MenuType.LobbyRoom);
            NGOMultiplayerManager.Instance.UpdateLobbyRoomUI();
        }
    }
}