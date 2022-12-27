using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using WAG.Multiplayer;

public class PlayerListItemUI : MonoBehaviour
{
    public string PlayerId { get; private set; }
    [SerializeField] private Button playerReadyButton;
    private TextMeshProUGUI playerNameText;
    private bool isReady = false;

    private void Awake()
    {
        if (!transform.Find("PlayerName").TryGetComponent(out playerNameText))
        {
            Debug.LogError($"Missing {playerNameText.GetType()} component on {this.name}");
        }
    }


    public void SetPlayerData(Player playerData)
    {
        playerNameText.SetText(playerData.Data["PlayerName"].Value);
        PlayerId = playerData.Id;
        if (PlayerId != AuthenticationService.Instance.PlayerId)
        {
            playerReadyButton.interactable = false;
            if (playerData.Data["Ready"].Value == "Y")
            {
                playerReadyButton.image.color = Color.green;
            }
        }

        
    }

    public void SetPlayerReady()
    {
        isReady = !isReady;
        if (isReady)
        {
            playerReadyButton.image.color = Color.green;
            LobbyManager.Instance.UpdatePlayerDataInCurrentLobby(new Dictionary<string, PlayerDataObject>
            {
                {
                    "Ready", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, value: "Y")
                }
            });
        }
        else
        {
            playerReadyButton.image.color = Color.white;
            LobbyManager.Instance.UpdatePlayerDataInCurrentLobby(new Dictionary<string, PlayerDataObject>
            {
                {
                    "Ready", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, value: "N")
                }
            });
        }
    }
}