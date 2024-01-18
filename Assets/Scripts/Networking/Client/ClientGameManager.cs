using System.Security.AccessControl;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Text;
using UnityEngine;
using Unity.Services.Core;
using UnityEngine.SceneManagement;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Debug = UnityEngine.Debug;
using Unity.Services.Authentication;
public class ClientGameManager : IDisposable
{
    private JoinAllocation allocation;
    private NetworkClient networkClient;
    private const string MenuSceneName = "Menu";
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);
        AuthState authState = await AuthenticationWrapper.DoAuth();
        if (authState == AuthState.Authenticated){
            return true;
        }
        return false;
    }

    public void GoToMenu(){
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string joinCode){
        try{
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch(Exception e){
            Debug.Log(e);
            return;
        }
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        UserData userData = new UserData{
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "MissingNo"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();
    }

    public void Dispose(){
        networkClient?.Dispose();
    }
}
