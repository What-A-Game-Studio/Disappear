using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LocalPlayerData
{
    private string playerName;

    public LocalPlayerData()
    {
        playerName = null;
    }

    public LocalPlayerData(string name)
    {
        playerName = name;
    }

    public Player GetLocalPlayerData()
    {
        return new Player(id: AuthenticationService.Instance.PlayerId, data: new Dictionary<string, PlayerDataObject>
        {
            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
        });
    }
}