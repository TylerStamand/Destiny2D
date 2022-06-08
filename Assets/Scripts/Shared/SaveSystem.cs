using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem {
    public static readonly string SaveFolder = Application.dataPath + "/Saves/";

    public static void Init() {
        if(!Directory.Exists(SaveFolder)) {
            Directory.CreateDirectory(SaveFolder);
        }
    }

    public static void SavePlayerData(List<PlayerData> clientData) {
       string json = JsonUtility.ToJson(clientData);
       File.WriteAllText(SaveFolder + "save.txt", json);
    }

    public static List<PlayerData> LoadPlayerData() {
        if(Directory.Exists(SaveFolder + "save.txt")) {
            string json = File.ReadAllText(SaveFolder + "save.txt");
            List<PlayerData> clientData = JsonUtility.FromJson<List<PlayerData>>(json);
            return clientData;
        }
        return null;
    }
}
