using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerState : NetworkBehaviour {
    public NetworkVariable<MovementStatus> MovementState {get;} = new NetworkVariable<MovementStatus>();

    
}


public enum MovementStatus {
    Idle,
    Walking
}