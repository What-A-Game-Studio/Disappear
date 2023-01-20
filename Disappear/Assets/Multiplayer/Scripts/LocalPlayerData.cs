using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LocalPlayerData
{
    public string playerId { get; private set; }
    public string playerName { get; private set; }
    public string playerReady { get; private set; }
    public string playerRole { get; private set; }

    public LocalPlayerData()
    {
        playerName = null;
        playerReady = "N";
        playerRole = "S";
    }

    public LocalPlayerData(string name, string ready = "N", string role = "S")
    {
        playerId = AuthenticationService.Instance.PlayerId;
        playerName = name;
        playerReady = ready;
        playerRole = role;
    }

    public Player GetLocalPlayerData()
    {
        return new Player(id: playerId, data: new Dictionary<string, PlayerDataObject>
        {
            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
            { "Ready", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerReady) },
            { "Role", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerRole) }
        });
    }
}