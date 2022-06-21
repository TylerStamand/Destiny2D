using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;


public class SaveSystem {
    public static readonly string SaveFolder = Application.dataPath + "/Saves/";

    public static void Init() {
        if(!Directory.Exists(SaveFolder)) {
            Directory.CreateDirectory(SaveFolder);
        }
    }

    public static void SavePlayerData(List<PlayerSaveData> playerSaveDataList) {
        string json = JsonConvert.SerializeObject(playerSaveDataList);
        File.WriteAllText(SaveFolder + "save.txt", json);
        Debug.Log("Saved Player Data");
    }

    public static PlayerSaveData LoadPlayerData(string playerID) {
     
        if(File.Exists(SaveFolder + "save.txt")) {
            string json = File.ReadAllText(SaveFolder + "save.txt");
            List<PlayerSaveData> playerSaveDataList = JsonConvert.DeserializeObject<List<PlayerSaveData>>(json);
            
            foreach(PlayerSaveData saveData in playerSaveDataList) {
                
                if(saveData.PlayerID == playerID) {
                    return  saveData;
                }
            } 
        }
        return null;
    }
}
