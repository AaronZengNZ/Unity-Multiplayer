using System.Globalization;
using System.ComponentModel;
using System.Timers;
using System.Threading;
using System.Runtime.CompilerServices;
//using System.Reflection.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private CoinWallet coinWallet;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed = 2f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float muzzleFlashDuration = 0.1f;
    [SerializeField] private int costToFire = 1;
    private bool shouldFire;
    private float timer;
    private float muzzleFlashTimer;
    public override void OnNetworkSpawn(){
        if(!IsOwner){return;}
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }
    public override void OnNetworkDespawn(){
        if(!IsOwner){return;}
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }
    // Update is called once per frame
    private void Update()
    {
        if(muzzleFlashTimer > 0f){
            muzzleFlashTimer -= Time.deltaTime;
            if(muzzleFlashTimer <= 0f){
                muzzleFlash.SetActive(false);
            }
        }
        if(!IsOwner){return;}
        if(timer > 0){
            timer -= Time.deltaTime;
        }
        if(!shouldFire){return;}
        if(timer > 0){
            return;
        }
        if(coinWallet.TotalCoins.Value < costToFire){return;}
        //set cost to fire to basecosttofire divided by basefirerate divided by firerate
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        timer = 1/fireRate;
    }
    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction){
        if(coinWallet.TotalCoins.Value < costToFire){return;}
        coinWallet.SpendCoins(costToFire);
        GameObject projectileInstance = Instantiate(
            serverProjectilePrefab, spawnPos, Quaternion.identity);
        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());
        SpawnDummyProjectileClientRpc(spawnPos, direction);
        if(projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage)){
            dealDamage.SetOwner(OwnerClientId);
        }
        if(projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)){
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction){
        if(IsOwner){return;}
        SpawnDummyProjectile(spawnPos, direction);
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction){
        muzzleFlash.SetActive(true);
        muzzleFlashTimer = muzzleFlashDuration;
        GameObject projectileInstance = Instantiate(
            clientProjectilePrefab, spawnPos, Quaternion.identity);
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());
        projectileInstance.transform.up = direction;
        if(projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)){
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }

    private void HandlePrimaryFire(bool shouldFire){
        this.shouldFire = shouldFire;
    }
}
