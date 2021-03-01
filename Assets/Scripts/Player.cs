using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public InputMaster controls;
    public LayerMask Ground;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    [SerializeField] float velocity = 0;
    const float ACCELERATION = 30f;
    const float DECELERATION = 50f;
    const float MAX_VELOCITY = 5f;
    const float JUMP_THRUST = 12f;
    bool facingRight = true;
    void Awake() {
        controls = new InputMaster();
        controls.Player.Jump.performed += ctx => Jump();
        rb = GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }
    void OnEnable() {
        controls.Enable();
    }
    void OnDisable() {
        controls.Disable();
    }
    void Decelerate() {
        if (velocity > .75f) {
            velocity -= DECELERATION * Time.fixedDeltaTime;
        }
        else if (velocity < -.75f) {
            velocity += DECELERATION * Time.fixedDeltaTime;
        }
        else {
            velocity = 0;
        }
    }
    private void Flip() {
        facingRight = !facingRight;
        transform.Rotate(0,180f,0);
    }
    bool IsGrounded() {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = .595f;

        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, Ground);
        if (hit.collider != null) {
            return true;
        }

        return false;
    }
    void Jump() {
        if (IsGrounded()) {
            rb.AddForce(new Vector2(0,JUMP_THRUST), ForceMode2D.Impulse);
        }
    }
    void Move(float direction) {
        if (direction == 0) {
            Decelerate();
        }
        else {
            if (Mathf.Abs(velocity) < MAX_VELOCITY)
                velocity += direction * ACCELERATION * Time.fixedDeltaTime;
        }

        rb.velocity = new Vector2(velocity, rb.velocity.y);
    }
    void Update() {
        Move(controls.Player.Move.ReadValue<float>());

        if (velocity > 0 && !facingRight) {
            Flip();
        }
        else if (velocity < 0 && facingRight) {
            Flip();
        }
    }
    
}