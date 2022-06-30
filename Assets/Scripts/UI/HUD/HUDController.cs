using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Globalization;

public class HUDController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;

    PlayerControllerServer playerServer;

    void Awake() {
        playerServer = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerControllerServer>();
        playerServer.Health.OnValueChanged += HandlePlayerHealthChange;
        HandlePlayerHealthChange(0, playerServer.Health.Value);
    }

    void HandlePlayerHealthChange(float prev, float current) {
        healthText.text = $"Health: {current.ToString("N", CultureInfo.CurrentCulture)}";
    }


}
