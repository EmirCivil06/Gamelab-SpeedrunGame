using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    private Rigidbody2D rb;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = .1f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement")]
    [SerializeField] private PlayerInput _input;
    [SerializeField] private InputAction _move, _jump;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private float coyoteTime = .1f; // çakılma süresi
    public float inputX;

    private float lastGroundCheckTime;
    private bool jumpQueued = false; // zıpalama aktive edildi mi kontrol ediyor
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInput>();
        
        
    }

    void OnEnable(){
        _move = InputSystem.actions.FindActionMap("Player").FindAction("Move");
        _jump = InputSystem.actions.FindActionMap("Player").FindAction("Jump"); 

        _move.Enable();
        _jump.Enable();

        _jump.started += OnJumpStarted;
        _jump.canceled += OnJumpCanceled;
    }

    void OnDisable(){
        _jump.started -= OnJumpStarted;
        _jump.canceled -= OnJumpCanceled;
        
        _move.Disable();
        _jump.Disable();
    }

    void OnJumpStarted(InputAction.CallbackContext context){
        jumpQueued = true;
        Debug.Log("Jump Queued");
    }

    void OnJumpCanceled(InputAction.CallbackContext context){
        if (rb.linearVelocity.y > 0f){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
        }
        Debug.Log("Jump Canceled");
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGrounded()){
            lastGroundCheckTime = Time.time;
        }
        if (jumpQueued && (IsGrounded() || Time.time - lastGroundCheckTime > coyoteTime)){
            PerformJump();
            jumpQueued = false;
            Debug.Log("Jump Queued over");
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        inputX = context.ReadValue<Vector2>().x;
        PerformMoving();
    }

    private void PerformMoving()
    {
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }
    private void FixedUpdate()
    {
        
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PerformJump();
            Debug.Log("Jumping action");
        }
    }
    private void PerformJump(){
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private bool IsGrounded(){
        if (!groundCheck) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }


}
