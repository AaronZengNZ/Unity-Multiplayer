using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class CoinWallet : NetworkBehaviour
{
    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();

    public void SpendCoins(int amount){
        TotalCoins.Value -= amount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int coinValue;
        if(other.TryGetComponent<Coin>(out Coin coin)){
            coinValue = coin.Collect();
        }else{return;}
        if(!IsServer) { return; }
        TotalCoins.Value += coinValue;
    }
}
