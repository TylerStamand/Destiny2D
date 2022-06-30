using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IDGrabberController : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button submitButton;

    void Awake() {
       // submitButton.onClick.AddListener(() => GameManager.Instance.SetPlayerID(inputField.text));
        inputField.onSubmit.AddListener(GameManager.Instance.SetPlayerID);
    }
}
