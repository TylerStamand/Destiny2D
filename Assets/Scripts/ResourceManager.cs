using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSystem {

    static private ResourceSystem instance;
    public static ResourceSystem Instance {
        get {
            if (instance == null) {
                instance = new ResourceSystem();
            }
            return instance;
        }
    }


    // public PlayerUnit Player { get; private set; }
    // public PlayerData PlayerData { get; private set; }
    // public CameraController PlayerCamera { get; private set; }

    // private Dictionary<EnemyType, Enemy> enemiesDict;
    // private Dictionary<WeaponType, Weapon> weaponsDict;
    private Dictionary<string, WeaponData> weaponDataDic;

    private ResourceSystem() {
        AssembleResources();
    }

    private void AssembleResources() {
        // Player = Resources.Load<PlayerUnit>("Player/Player");
        // PlayerData = Resources.Load<PlayerData>("Player/PlayerData");
        // PlayerCamera = Resources.Load<CameraController>("Player/PlayerCamera");

        // List<Enemy> Enemies = Resources.LoadAll<Enemy>("Enemies").ToList();
        // enemiesDict = Enemies.ToDictionary(r => r.EnemyType, r => r);

        // List<Weapon> Weapons = Resources.LoadAll<Weapon>("Weapons").ToList();
        // weaponsDict = Weapons.ToDictionary(r => r.WeaponType, r => r);

        List<WeaponData> weaponDataList = Resources.LoadAll<WeaponData>("Items/Weapons").ToList();
    
        weaponDataDic = weaponDataList.ToDictionary(r => {
            if(r.Name != WeaponData.DefaultName) {
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

    // public Enemy GetEnemy(EnemyType t) => enemiesDict[t];
    // public Weapon GetWeapon(WeaponType t) => weaponsDict[t];
    public WeaponData GetWeaponData(string name) {
        if(weaponDataDic.TryGetValue(name, out WeaponData weaponData)) {
            return weaponData;
        }
        else {
            Debug.LogError($"No weapon of name: {name} was found");
            return null;
        }

    } 
}