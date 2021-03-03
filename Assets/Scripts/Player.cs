using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    public InputMaster controls;
    public LayerMask Ground;
    public LayerMask Wall;
    Rigidbody2D rb;
    SpriteRenderer sprite;
    [SerializeField] Vector2 velocity = new Vector2(0f, 0f);
    float acceleration = 90f;
    float midairAcceleration = 22f;
    float deceleration = 40f;
    float midairDeceleration = 9f;
    float maxVelocity = 5f;
    float jumpVelocity = 12f;
    float wallJumpVelocity = 6f;
    [SerializeField] public bool hoverUnlocked = false;
    private bool _facingRight = true;
    private bool _hoverActive = false;
    void Awake() {
        controls = new InputMaster();
        controls.Player.Jump.performed += _ => Jump();
        controls.Player.Hover.performed += ctx => _hoverActive = true;
        controls.Player.Hover.canceled += ctx => _hoverActive = false;
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
        _facingRight = !_facingRight;
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
            velocity.y = jumpVelocity;
            rb.velocity = new Vector2(rb.velocity.x, velocity.y);
        }
        else if (OnWall() != 0) {
            WallJump(new Vector2(wallJumpVelocity * OnWall(), jumpVelocity / 1.5f));
        }
    }
    void Hover() {
        if (hoverUnlocked) {
            velocity.y = .4f;
            rb.velocity = velocity;
        }
    }
    void Move(float direction) {
        if (Mathf.Abs(velocity.x) < maxVelocity) {
            if (!IsGrounded()) {
                velocity.x += direction * midairAcceleration * Time.fixedDeltaTime;
            }
            else {
                velocity.x += direction * acceleration * Time.fixedDeltaTime;
            }
        }
        if (!IsGrounded()) {
            Decelerate(midairDeceleration);
        }
        else {
            Decelerate(deceleration);
        }
        rb.velocity = new Vector2(velocity.x, rb.velocity.y);
    }
    void Update() {
        Move(controls.Player.Move.ReadValue<float>());
        if (!IsGrounded() && _hoverActive) {
            Hover();
        }
        if ((velocity.x > 0 && !_facingRight) ||
            (velocity.x < 0 && _facingRight)) {
            Flip();
        }
    }
    
}