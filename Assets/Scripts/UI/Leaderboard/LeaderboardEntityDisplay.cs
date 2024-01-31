using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;
using TMPro;


public class LeaderboardEntityDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private Color myColour;

    public ulong ClientId {get; private set;}
    private FixedString32Bytes playerName;
    public int Coins {get; private set;}

    public void Initialise(ulong clientId, FixedString32Bytes playerName, int coins){
        ClientId = clientId;
        this.playerName = playerName;

        if(ClientId == NetworkManager.Singleton.LocalClientId){
            displayText.color = myColour;
        }

        UpdateCoins(coins);
    }

    public void UpdateCoins(int coins){
        Coins = coins;
        UpdateText();
    }

    public void UpdateText(){
        displayText.text = $"{transform.GetSiblingIndex() + 1}. {playerName} - {Coins}";
    }
}
