using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerController : MonoBehaviour { 

    [SerializeField] float moveForce;
    [SerializeField] float topSpeed;
    [SerializeField] MeleeWeapon weapon;
    new Rigidbody2D rigidbody;
    Animator animator;

    public Vector2 Direction {get; private set;} 

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rigidbody.gravityScale = 0;
        Direction = new Vector2(0, -1);
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            weapon.Attack(Direction);
        }
    }

    void FixedUpdate() {

        //Currently no smoothing is applied, change later if needed
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        animator.SetFloat("X", horizontal);
        animator.SetFloat("Y", vertical);

        if(horizontal !=0 || vertical != 0) {

            Direction = new Vector2(horizontal, vertical);
        }
        //normalized to prevent moving quicker diagonally
        Vector2 inputMovement = new Vector2(horizontal, vertical).normalized;

        inputMovement.x *= moveForce * Time.deltaTime;
        inputMovement.y *= moveForce * Time.deltaTime;

        rigidbody.velocity = transform.right * inputMovement.x + transform.up * inputMovement.y;
    
        

        
    }

}
