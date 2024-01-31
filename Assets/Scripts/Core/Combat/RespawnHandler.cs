using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private PlayerSetup playerPrefab;
    [SerializeField] private float keptCoinPercentage = 75f;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        PlayerSetup[] players = FindObjectsByType<PlayerSetup>(FindObjectsSortMode.None);
        foreach (PlayerSetup player in players)
        {
            HandlePlayerSpawned(player);
        }

        PlayerSetup.OnPlayerSpawned += HandlePlayerSpawned;
        PlayerSetup.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        PlayerSetup.OnPlayerSpawned -= HandlePlayerSpawned;
        PlayerSetup.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(PlayerSetup player)
    {
        player.Health.OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawned(PlayerSetup player)
    {
        player.Health.OnDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(PlayerSetup player)
    {
        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId)
    {
        yield return null;

        PlayerSetup playerInstance = Instantiate(
            playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        float keptCoins = playerInstance.Wallet.TotalCoins * keptCoinPercentage/100;
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);
    }
}
