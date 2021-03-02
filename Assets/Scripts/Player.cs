using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public InputMaster controls;
    public LayerMask Ground;
    public LayerMask Wall;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    [SerializeField] float velocity = 0;
    const float ACCELERATION = 100f;
    const float DECELERATION = 50f;
    const float MAX_VELOCITY = 5f;
    const float JUMP_THRUST = 8f;
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
    bool IsOnRightWall() {
        Vector2 position = transform.position;
        float distance = .57f;

        Debug.DrawRay(position, Vector2.right, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.right, distance, Wall);
        if (hit.collider) {
            Debug.Log("On right wall!");
            return true;
        }

        return false;
    }
    bool IsOnLeftWall() {
        Vector2 position = transform.position;
        float distance = .57f;

        Debug.DrawRay(position, Vector2.left, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.left, distance, Wall);
        if (hit.collider) {
            Debug.Log("On left wall!");
            return true;
        }

        return false;
    }
    void WallJump(float jumpVelocity) {
        rb.AddForce(new Vector2(0,JUMP_THRUST), ForceMode2D.Impulse);
        velocity = jumpVelocity;
    }
    void Jump() {
        if (IsGrounded()) {
            rb.AddForce(new Vector2(0,JUMP_THRUST), ForceMode2D.Impulse);
        }
        else if (IsOnRightWall()) {
            WallJump(-8f);
        }
        else if (IsOnLeftWall()) {
            WallJump(8f);
        }
    }
    void Move(float direction) {
        if (Mathf.Abs(velocity) < MAX_VELOCITY) {
            velocity += direction * ACCELERATION * Time.fixedDeltaTime;
        }
        Decelerate();

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