using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    private Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer SpriteRenderer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private bool isGrounded;

    [Header("Movement")]
    [SerializeField] private PlayerInput _input;
    [SerializeField] private InputAction _move, _jump;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private float coyoteTime = .8f; // çakılma süresi
    public float inputX;

    private float lastGroundCheckTime;
    private int jumpCount; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        
        
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
        //Debug.Log("Jump Queued");
    }

    void OnJumpCanceled(InputAction.CallbackContext context){
        if (rb.linearVelocity.y > 0f){
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGrounded()){
            lastGroundCheckTime = Time.time;
            //Debug.Log("Grounded");
        }
        if (jumpCount!=0 && IsGrounded()){
            jumpCount = 0;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        inputX = context.ReadValue<Vector2>().x;
        PerformMoving();
    }

    private void PerformMoving()
    {
        if (inputX < 0){
            SpriteRenderer.flipX = false;
        }
        else if (inputX > 0){
            SpriteRenderer.flipX = true;
        }
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
        animator.SetFloat("HorizontalMove", Mathf.Abs(inputX));
    }
    private void FixedUpdate()
    {
        
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Double jump sadece coyote time içersinde aktif oluyor. uzun süre beklenirse double jump yapılamıyor.
        if (context.performed && Time.time - lastGroundCheckTime < coyoteTime && jumpCount < 2)
        {
            PerformJump();
            jumpCount++;
            Debug.Log("Jumping action");
        }
    }
    private void PerformJump(){
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    
    public bool IsGrounded(){
        if (!groundCheck) return false;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }


}
