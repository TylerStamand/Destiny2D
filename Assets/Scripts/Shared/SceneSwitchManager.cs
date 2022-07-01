using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SceneSwitchManager
{
    public enum SceneStates {
        Init, MainMenu, InGame
    }
    
    public SceneStates SceneState {get; private set;}

    public static SceneSwitchManager Instance {get; private set;}

    public event Action<SceneStates> OnSceneStateChanged; 
    public event Action<ulong> OnClientLoadedScene;


    

    // private void Awake() {
    //     if (Instance != this && Instance != null) {
    //         GameObject.Destroy(Instance.gameObject);
    //     }
    //     Instance = this;
    //     SetSceneState(SceneStates.Init);
    // }

    public void SetSceneState(SceneStates sceneState) {
        SceneState = sceneState;
        OnSceneStateChanged?.Invoke(sceneState);

    }

    public void SwitchScene(string sceneName) {
        if (NetworkManager.Singleton.IsListening) {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        else {
            SceneManager.LoadSceneAsync(sceneName);
        }
    }

    private void OnSceneEvent(SceneEvent sceneEvent) {
        //We are only interested by Client Loaded Scene events
        if (sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;

        OnClientLoadedScene?.Invoke(sceneEvent.ClientId);
    }
    
}
