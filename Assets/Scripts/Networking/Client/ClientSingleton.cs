using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public class ClientSingleton : MonoBehaviour
{
    private static ClientSingleton instance;
    public ClientGameManager GameManager {get; private set;}
    public static ClientSingleton Instance{
        get{
            if(instance != null){return instance;}
            instance = FindObjectOfType<ClientSingleton>();
            if(instance == null){
                UnityEngine.Debug.LogError("No ClientSingleton found in scene!"); 
                return null;
            }
            return instance;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CreateClient(){
        GameManager = new ClientGameManager();
        return await GameManager.InitAsync();
    }
}
