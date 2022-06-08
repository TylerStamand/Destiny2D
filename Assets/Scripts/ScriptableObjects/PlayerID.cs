using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerID", menuName = "ScriptableObjects/Player")]
public class PlayerID : ScriptableObject {
    public Guid ID; 
}
