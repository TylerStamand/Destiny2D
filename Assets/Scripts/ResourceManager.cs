using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;



public class ResourceManager {

    public static DropServer DropPrefab {get; private set;}

    static private ResourceManager instance;
    public static ResourceManager Instance {
        get {
            if (instance == null) {
                instance = new ResourceManager();
            }
            return instance;
        }
    }


    // public PlayerUnit Player { get; private set; }
    // public PlayerData PlayerData { get; private set; }
    // public CameraController PlayerCamera { get; private set; }

    // private Dictionary<EnemyType, Enemy> enemiesDict;
    // private Dictionary<WeaponType, Weapon> weaponsDict;
    private Dictionary<string, ItemData> itemDataDic;
    private Dictionary<Guid, PlayerData> playerDataDic;

    private ResourceManager() {
        AssembleResources();
    }

    private void AssembleResources() {

        DropPrefab = Resources.Load<DropServer>("Prefabs/Drop");

        // List<PlayerData> playerDataList = Resources.LoadAll<PlayerData>("Players").ToList();
        List<ItemData> itemDataList = Resources.LoadAll<ItemData>("Items").ToList();
    
        // playerDataDic = playerDataList.ToDictionary(player => player.playerGUID, player => player);

        itemDataDic = itemDataList.ToDictionary(r => {
            if(r.Name != ItemData.DefaultName) {
                return r.Name;
            }
            else {
                Debug.LogError("Weapon not given a name");
                return "";
            }
        },    
            r => r
        );
    }


    public ItemData GetItemData(string name) {
        if(itemDataDic.TryGetValue(name, out ItemData itemData)) {
            return itemData;
        }
        else {
            Debug.LogError($"No item of name: {name} was found");
            return null;
        }

    } 


    public PlayerData GetPlayerData(Guid playerGUID) {
        return playerDataDic[playerGUID];
    }
}