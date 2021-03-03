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
    [SerializeField] Vector2 velocity = new Vector2(0f, 0f);
    const float ACCELERATION = 90f;
    const float MIDAIR_ACCELERATION = 22f;
    const float DECELERATION = 40f;
    const float MIDAIR_DECELERATION = 9f;
    const float MAX_VELOCITY = 5f;
    const float JUMP_VELOCITY = 12f;
    const float WALL_JUMP_VELOCITY = 6f;
    bool facingRight = true;
    private bool _hover = false;
    void Awake() {
        controls = new InputMaster();
        controls.Player.Jump.performed += _ => Jump();
        controls.Player.Hover.performed += ctx => _hover = true;
        controls.Player.Hover.canceled += ctx => _hover = false;
        rb = GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }
    void OnEnable() {
        controls.Enable();
    }
    void OnDisable() {
        controls.Disable();
    }
    void Decelerate(float deceleration) {
        if (velocity.x > .4f) {
            velocity.x -= deceleration * Time.fixedDeltaTime;
        }
        else if (velocity.x < -.4f) {
            velocity.x += deceleration * Time.fixedDeltaTime;
        }
        else {
            velocity.x = 0;
        }
    }
    private void Flip() {
        facingRight = !facingRight;
        transform.Rotate(0,180f,0);
    }
    bool IsGrounded() {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = .4f;

        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, Ground);
        if (hit.collider != null) {
            return true;
        }

        return false;
    }
    int OnWall() {
        Vector2 position = transform.position;
        float distance = .4f;

        RaycastHit2D hitRight = Physics2D.Raycast(position, Vector2.right, distance, Wall);
        RaycastHit2D hitLeft = Physics2D.Raycast(position, Vector2.left, distance, Wall);
        if (hitRight.collider) {
            return -1;
        }
        else if (hitLeft.collider) {
            return 1;
        }

        return 0;
    }
    void WallJump(Vector2 jumpVelocity) {
        velocity = jumpVelocity;
        rb.velocity = jumpVelocity;
    }
    void Jump() {
        if (IsGrounded()) {
            velocity.y = JUMP_VELOCITY;
            rb.velocity = new Vector2(rb.velocity.x, velocity.y);
        }
        else if (OnWall() != 0) {
            WallJump(new Vector2(WALL_JUMP_VELOCITY * OnWall(), JUMP_VELOCITY / 1.5f));
        }
    }
    void Hover() {
        velocity.y = .4f;
        rb.velocity = velocity;
    }
    void Move(float direction) {
        if (Mathf.Abs(velocity.x) < MAX_VELOCITY) {
            if (!IsGrounded()) {
                velocity.x += direction * MIDAIR_ACCELERATION * Time.fixedDeltaTime;
            }
            else {
                velocity.x += direction * ACCELERATION * Time.fixedDeltaTime;
            }
        }
        if (!IsGrounded()) {
            Decelerate(MIDAIR_DECELERATION);
        }
        else {
            Decelerate(DECELERATION);
        }
        rb.velocity = new Vector2(velocity.x, rb.velocity.y);
    }
    void Update() {
        Move(controls.Player.Move.ReadValue<float>());
        if (!IsGrounded() && _hover) {
            Hover();
        }
        if (velocity.x > 0 && !facingRight) {
            Flip();
        }
        else if (velocity.x < 0 && facingRight) {
            Flip();
        }
    }
    
}