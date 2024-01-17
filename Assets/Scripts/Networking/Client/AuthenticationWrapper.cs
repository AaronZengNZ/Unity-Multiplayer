using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;

public static class AuthenticationWrapper
{
    public static AuthState AuthState {get; private set;} = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxTries = 5){
        if(AuthState == AuthState.Authenticated){
            return AuthState;
        }

        if(AuthState == AuthState.Authenticating){
            UnityEngine.Debug.LogWarning("Already authenticating!");
            await Authenticating();
            return AuthState;
        }
        await SignInAnonymouslyAsync(maxTries);
        return AuthState;
    }

    private static async Task<AuthState> Authenticating(){
        while(AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated){
            await Task.Delay(200);
        }
        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maxTries){
        AuthState = AuthState.Authenticating;
        int tries = 0;
        while(AuthState == AuthState.Authenticating && tries < maxTries){
            try {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                if(AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized){
                    AuthState = AuthState.Authenticated;
                    break;
                }
            }
            catch(AuthenticationException authenticationException){
                UnityEngine.Debug.LogError(authenticationException);
                AuthState = AuthState.Error;
            }
            catch(RequestFailedException requestFailedException){
                UnityEngine.Debug.LogError(requestFailedException);
                AuthState = AuthState.Error;
            }
            
            tries++;
            await Task.Delay(1000);
        }

        if(AuthState != AuthState.Authenticated){
            UnityEngine.Debug.LogWarning($"Player was not signed in successfully after {tries} tries");
            AuthState = AuthState.Timeout;
        }
    }
}



public enum AuthState{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    Timeout
}