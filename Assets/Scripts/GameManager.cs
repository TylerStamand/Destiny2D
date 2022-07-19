using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public enum GameState {
    MainMenu,
    Play
}

public class GameManager : NetworkBehaviour {

    [SerializeField] GameObject PlayerUnit;
    [SerializeField] GameObject IDGrabberUI;

    [SerializeField] Enemy defaultEnemyPrefab;
    [SerializeField] WeaponData weaponDrop;


    public event Action<ulong> OnNetListFixFinished;

    public static GameManager Instance;

    public NetworkVariable<GameState> CurrentGameState;

    public bool playerIDSet = false;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
        GameObject idGrabberInstance;
#endif



    Dungeon currentDungeon;
    List<ConnectedPlayer> connectedPlayers = new List<ConnectedPlayer>();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }



        //Come back to this. Initialization probably shouldn't happen here
        SaveSystem.Init();
        //sessionManager = SessionManager.Instance;
    }

    void Start() {

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        idGrabberInstance = Instantiate(IDGrabberUI);
#endif
       
    }

    void Update() {
        if (!IsSpawned) return;
        if (Input.GetKeyDown(KeyCode.F)) {
            SavePlayerData();
        }

    }

    //Handlers for Main Menu
    public void StartGame() {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.StartHost();
        
    }

    public void JoinGame() {
        NetworkManager.StartClient();
    }



    public void SetPlayerID(string playerID) {

#if DEVELOPMENT_BUILD || UNITY_EDITOR
       Destroy(idGrabberInstance);
#endif
        playerIDSet = true;
        PlayerPrefs.SetString("PlayerID", playerID);

        SceneManager.LoadScene("MainMenu");
        CurrentGameState.Value = GameState.MainMenu;
    }


    void OnGUI() {

        if (IsSpawned) {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (GUILayout.Button("Spawn Enemy")) {
                Vector3 spawn = SpawnManager.Instance.GetSpawnLocation().transform.position;
                Enemy enemy = Instantiate(defaultEnemyPrefab, spawn, Quaternion.identity);
                enemy.NetworkObject.Spawn();
            }
            if (GUILayout.Button("Spawn Item")) {
                DropServer spawnWeaponDrop = Instantiate(ResourceManager.Instance.DropPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                spawnWeaponDrop.SetItem(weaponDrop.CreateItem());
                spawnWeaponDrop.GetComponent<NetworkObject>().Spawn();
            }
            GUILayout.EndArea();
        }


    }
    public void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;

        // Your approval logic determines the following values
        response.Approved = true;
        response.CreatePlayerObject = true;

        // The prefab hash value of the NetworkPrefab, if null the default NetworkManager player prefab is used
        response.PlayerPrefabHash = null;

        // Position to spawn the player object (if null it uses default of Vector3.zero)

        response.Position = Vector3.zero;

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if (!IsServer) {
            enabled = false;
            return;
        }

        Debug.Log("GameManager Spawn");

        NetworkManager.SceneManager.LoadScene("Dungeon", LoadSceneMode.Single);
        NetworkManager.SceneManager.OnLoadComplete += InitializeDungeon;
        


        NetworkManager.SceneManager.OnSceneEvent += SceneEvent;


        if (IsServer && IsClient) {
            PlayerConnected(NetworkObject.OwnerClientId);
        }

        NetworkManager.OnClientConnectedCallback += PlayerConnected;


        

    }



    void SceneEvent(SceneEvent sceneEvent) {
        if(sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
        if(sceneEvent.ClientId != 0) {
            PlayerConnected(sceneEvent.ClientId);
        }
    }

    void InitializeDungeon(ulong clientID, string sceneName, LoadSceneMode loadSceneMode) {
        if(!IsServer) return;
        currentDungeon = FindObjectOfType<Dungeon>();
        currentDungeon.OnStairsEntered += SetUpDungeon;
        SetUpDungeon();
    }


    void SetUpDungeon() {
        currentDungeon.GenerateMap();
        List<Vector2> spawnPositions = currentDungeon.GetSpawnPositions();
        List<PlayerControllerServer> players = FindObjectsOfType<PlayerControllerServer>().ToList();
        foreach(PlayerControllerServer player in players) {
            Destroy(player.gameObject); 
        }
        for(int i = 0; i < connectedPlayers.Count; i++) {
            
            SpawnPlayer(connectedPlayers[i].GetComponent<NetworkObject>().OwnerClientId, spawnPositions[i]);

        }
        
    }



    void SpawnPlayer(ulong clientID, Vector2 spawnPosition) {
        if(!IsServer) return;
        GameObject playerUnitInstance = Instantiate(PlayerUnit, spawnPosition, Quaternion.identity);
        NetworkObject playerUnitNetObj = playerUnitInstance.GetComponent<NetworkObject>();
        playerUnitNetObj.SpawnAsPlayerObject(clientID);

        //sessionManager.SetupConnectingPlayerSessionData(clientID, playerID);
        string playerID = PlayerPrefs.GetString("PlayerID");
        PlayerSaveData saveData = SaveSystem.LoadPlayerData(playerID);

        if (saveData != null) {
            playerUnitInstance.GetComponent<PlayerControllerServer>().SetSaveDataServer(saveData);
        }
    }

    void PlayerConnected(ulong clientID) {
        Debug.Log("Player Connected");
     
        //Might need to set destroy it with scene depending

        NetworkObject playerNetObj = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);
        
        ConnectedPlayer connectedPlayer = playerNetObj.GetComponent<ConnectedPlayer>();
        connectedPlayers.Add(connectedPlayer);
        // connectedPlayer.ClientID.Value = clientId;
        // connectedPlayer.PlayerUnitObjectNetID.Value = playerUnitNetObj.NetworkObjectId;




        if(!playerNetObj.IsLocalPlayer)
            StartCoroutine(FixNetList(playerNetObj));

    }

    void SavePlayerData() {
        List<PlayerControllerServer> players = GameObject.FindObjectsOfType<PlayerControllerServer>().ToList();
        List<PlayerSaveData> playerSaveDataList = new List<PlayerSaveData>();
        foreach (PlayerControllerServer player in players) {
            playerSaveDataList.Add(player.GetSaveDataServer());
        }
        SaveSystem.SavePlayerData(playerSaveDataList);
    }

    IEnumerator FixNetList(NetworkObject player) {
        Debug.Log("Fixing Net List");
        yield return new WaitForSeconds(0.001f);
        player.NetworkHide(player.OwnerClientId);
        yield return new WaitForSeconds(0.001f);
        player.NetworkShow(player.OwnerClientId);
        Debug.Log("Finished Net List Fix");
    }

}


