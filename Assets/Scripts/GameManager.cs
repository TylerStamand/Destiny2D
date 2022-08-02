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

    public List<ConnectedPlayer> ConnectedPlayers { get; private set; } = new List<ConnectedPlayer>();

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        GameObject idGrabberInstance;
#endif


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
            if (GUILayout.Button("Spawn Item")) {
                DropServer spawnWeaponDrop = Instantiate(ResourceManager.Instance.DropPrefab, NetworkManager.LocalClient.PlayerObject.transform.position, Quaternion.identity);
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
        //Bad
        NetworkManager.SceneManager.OnLoadComplete += HandleLoadComplete;
        


        NetworkManager.SceneManager.OnSceneEvent += SceneEvent;


        //if (IsServer && IsClient) {
        //    PlayerConnected(NetworkObject.OwnerClientId);
        //}

        //NetworkManager.OnClientConnectedCallback += PlayerConnected;


        

    }



    void SceneEvent(SceneEvent sceneEvent) {
        if(sceneEvent.SceneEventType != SceneEventType.LoadComplete) return;
        PlayerConnected(sceneEvent.ClientId);
        
    }

    void HandleLoadComplete(ulong clientID, string sceneName, LoadSceneMode loadSceneMode) {
        //Bad Code find better way to handle this
        if(sceneName == "Dungeon") {
            FindObjectOfType<DungeonManager>().LoadPlayerIntoDungeon(clientID);
        }
    }




    //This should probably be in a different class with managing connections but this is fine for the time being.
    public void SpawnPlayer(ulong clientID, Vector2 spawnPosition) {
        if(!IsServer) return;
        GameObject playerUnitInstance = Instantiate(PlayerUnit, spawnPosition, Quaternion.identity);
        NetworkObject playerUnitNetObj = playerUnitInstance.GetComponent<NetworkObject>();
        playerUnitNetObj.SpawnAsPlayerObject(clientID);
        


        //sessionManager.SetupConnectingPlayerSessionData(clientID, playerID);7
        string playerID = PlayerPrefs.GetString("PlayerID");
        PlayerSaveData saveData = SaveSystem.LoadPlayerData(playerID);

        if (saveData != null) {
            playerUnitInstance.GetComponent<PlayerControllerServer>().SetSaveDataServer(saveData);
        }
        
        if(!playerUnitNetObj.IsLocalPlayer)
            StartCoroutine(FixNetList(playerUnitNetObj));
    }

    void PlayerConnected(ulong clientID) {
        Debug.Log("Player Connected");

        NetworkObject playerNetObj = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientID);
        
        ConnectedPlayer connectedPlayer = playerNetObj.GetComponent<ConnectedPlayer>();
        ConnectedPlayers.Add(connectedPlayer);
        // connectedPlayer.ClientID.Value = clientId;
        // connectedPlayer.PlayerUnitObjectNetID.Value = playerUnitNetObj.NetworkObjectId;





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
        yield return new WaitForSeconds(0.001f);
        player.NetworkHide(player.OwnerClientId);
        yield return new WaitForSeconds(0.001f);
        player.NetworkShow(player.OwnerClientId);
    }


}


