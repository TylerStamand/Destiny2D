using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamTest : MonoBehaviour {
    void Awake() {
        if(!SteamManager.Initialized) return;

        string name = SteamFriends.GetPersonaName();
        Debug.Log(name);
    }
}