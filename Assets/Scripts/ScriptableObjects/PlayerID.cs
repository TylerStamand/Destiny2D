using System;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "PlayerID", menuName = "ScriptableObjects/Player")]
public class PlayerID : SerializedScriptableObject {
    public Guid ID; 
}
