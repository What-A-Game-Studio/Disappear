using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WAG.Core.GM;
using WAG.Menu;

namespace WAG.Multiplayer
{
    public class LobbyCreationUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField lobbyNameInputField;
        [SerializeField] private TextMeshProUGUI maxPlayersText;
        [SerializeField] private Toggle isPrivate;
        private int maxPlayers;

        private void Awake()
        {
            maxPlayers = 2;
            maxPlayersText.text = maxPlayers.ToString();
        }

        public void MaxPlayerMinusOnClick()
        {
            maxPlayers = Mathf.Clamp(--maxPlayers, 2, 10);
            maxPlayersText.text = maxPlayers.ToString();
        }

        public void MaxPlayerPlusOnClick()
        {
            maxPlayers = Mathf.Clamp(++maxPlayers, 2, 10);
            maxPlayersText.text = maxPlayers.ToString();
        }

        public async void CreateLobbyRequest()
        {
            NGOMultiplayerManager.Instance.CreateLobby(lobbyNameInputField.text,
                maxPlayers,
                isPrivate.isOn);
            MenuManager.Instance.OpenMenu(MenuType.LobbyRoom);
        }
    }
}