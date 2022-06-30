using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuController : MonoBehaviour
{
    [SerializeField] Button StartButton;
    [SerializeField] Button JoinButton;
    

    void Awake() {
        StartButton.onClick.AddListener(GameManager.Instance.StartGame);
        JoinButton.onClick.AddListener(GameManager.Instance.JoinGame);
    }
}
