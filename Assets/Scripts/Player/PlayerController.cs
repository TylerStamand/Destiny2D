using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerController : MonoBehaviour { 

    [SerializeField] float moveForce;
    [SerializeField] float topSpeed;
    new Rigidbody2D rigidbody;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0;
    }

    void FixedUpdate() {

        //Currently no smoothing is applied, change later if needed
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //normalized to prevent moving quicker diagonally
        Vector2 inputMovement = new Vector2(horizontal, vertical).normalized;

        inputMovement.x *= moveForce * Time.deltaTime;
        inputMovement.y *= moveForce * Time.deltaTime;

        rigidbody.velocity = transform.right * inputMovement.x + transform.up * inputMovement.y;
    
        

        
    }

}
