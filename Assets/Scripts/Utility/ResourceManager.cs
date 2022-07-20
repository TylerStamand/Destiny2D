using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Unity.Netcode;


public class ResourceManager {

    public DropServer DropPrefab {get; private set;}
    public NetworkObject NetworkParentPrefab {get; private set;}
    public CameraFollower CameraFollowerPrefab {get; private set;}

    static private ResourceManager instance;
    public static ResourceManager Instance {
        get {
            if (instance == null) {
                instance = new ResourceManager();
            }
            return instance;
        }
    }


    
    private Dictionary<string, ItemData> itemDataDic;

    private ResourceManager() {
        AssembleResources();
    }

    private void AssembleResources() {

        DropPrefab = Resources.Load<DropServer>("Prefabs/Drop");
        NetworkParentPrefab = Resources.Load<NetworkObject>("Prefabs/NetworkParent");
        CameraFollowerPrefab = Resources.Load<CameraFollower>("Prefabs/CameraFollower");

        List<ItemData> itemDataList = Resources.LoadAll<ItemData>("Items").ToList();
    

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
        itemDataDic.TryGetValue(name, out ItemData value);
        return value;

    } 


}