using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyPlayersText;
    private LobbiesList lobbiesList;
    private Lobby lobby;

    public void Initialise(LobbiesList lobbiesList, Lobby lobby){
        this.lobbiesList = lobbiesList;
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        lobbyPlayersText.text = $"Players: {lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void Join(){
        lobbiesList.JoinAsync(lobby);
    }
}
