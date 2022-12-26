using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerListItemUI : MonoBehaviour
{
    private TextMeshProUGUI playerNameText; 
    public string PlayerId { get; private set; }
    
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
    }
}
