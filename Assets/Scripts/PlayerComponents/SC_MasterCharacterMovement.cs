using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_MasterCharacterMovement : MonoBehaviour
{
    private PlayerInput controls;
    
    [Header("Movement")]
    private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runMultiplier; 
    private bool runActive = false;
    
    [SerializeField] private float gravityScale = 20f;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airMultiplier;

    [SerializeField] private float rotationSpeed = 10f;
    public Vector2 moveInput;
    public Vector3 moveDirection;
    
    [Header("Jumping")]
    bool readyToJump;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private int doubleJump;
    private int doubleJumpsLeft;

    [Header("Crouch")] 
    [SerializeField] private float crouchMultiplier;
    [SerializeField] private float crouchYScale;
    private bool crouchActive = false;

    [Header("Dash")]
    [SerializeField] private float maxDashTime = 0.3f;
    [SerializeField] private float dashForce = 12f; 
    [SerializeField] private float dashCooldown = 1f;
    private float dashTimer; // tiempo que dura el dash
    private bool readyToDash = true;
    private bool dashActive;
    private Vector3 dashDirection; // dirección bloqueada del dash

    [Header("Ground Check")]
    [SerializeField] private Transform feet;
    [SerializeField] private float detectionRadius = 0.3f;
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform orientation;
    private bool grounded;
    
    Rigidbody rb;
    
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    
    public MovementState courrentState;
    public enum MovementState
    { walking, running, crouching, dashing, air }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        controls.actions["moveInput"].performed += MoveAction;
        controls.actions["moveInput"].canceled += MoveAction;
        controls.actions["jumpInput"].started += JumpAction;
        controls.actions["runInput"].started += RunStartedAction;
        controls.actions["runInput"].canceled += RunCanceledAction;
        controls.actions["crouchInput"].started += CrouchStartedAction;
        controls.actions["crouchInput"].canceled += CrouchCanceledAction;
        controls.actions["dashInput"].started += DashAction;
    }

    private void OnDisable()
    {
        controls.actions["moveInput"].performed -= MoveAction;
        controls.actions["moveInput"].canceled -= MoveAction;
        controls.actions["jumpInput"].started -= JumpAction;
        controls.actions["runInput"].started -= RunStartedAction;
        controls.actions["runInput"].canceled -= RunCanceledAction;
        controls.actions["crouchInput"].started -= CrouchStartedAction;
        controls.actions["crouchInput"].canceled -= CrouchCanceledAction;
        controls.actions["dashInput"].started -= DashAction;
    }

    private void MoveAction(InputAction.CallbackContext obj)
    {
        // aquí solo guardamos el input
        // la dirección real se recalcula continuamente en FixedUpdate
        moveInput = obj.ReadValue<Vector2>();
    }

    private void JumpAction(InputAction.CallbackContext obj)
    {
        if (readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        else if(!grounded)
        {
            DoubleJump();
        }
    }

    private void RunStartedAction(InputAction.CallbackContext obj)
    {
        runActive = true;
    }

    private void RunCanceledAction(InputAction.CallbackContext obj)
    {
        runActive = false;
    }
    
    private void CrouchStartedAction(InputAction.CallbackContext obj)
    {
        if (grounded)
        {
            crouchActive = true;
        }
    }

    private void CrouchCanceledAction(InputAction.CallbackContext obj)
    {
        crouchActive = false;
    }
    
    private void DashAction(InputAction.CallbackContext obj)
    {
        StartDash();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        moveSpeed = walkSpeed;
        rb.freezeRotation = true;
        readyToJump = true;
    }

    private void Update()
    {
        GroundCheck();

        if (grounded && doubleJumpsLeft != doubleJump)
        {
            ResetDoubleJumps();
        }

        StateHandler();
        HandleDrag();

        if (dashActive) { HandleDashTimer(); }
    }

    private void FixedUpdate()
    {
        ApplyGravity();

        HandleRotation();

        // como el player rota con la cámara,
        // recalculamos la dirección cada frame con su forward actual
        moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;

        if (dashActive)
        {
            DashMovement();
        }
        else
        {
            Movement();
        }

        SpeedControl();
    }

    private void GroundCheck()
    {
        grounded = Physics.CheckSphere(feet.position, detectionRadius, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        if (feet != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(feet.position, detectionRadius);
        }
    }
    
    private void ApplyGravity()
    {
        if (grounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -2f, rb.linearVelocity.z);
        }
        else
        {
            rb.AddForce(Vector3.down * gravityScale, ForceMode.Force);
        }
    }

    private void StateHandler()
    {
        if (dashActive)
            courrentState = MovementState.dashing;
        else if (crouchActive)
            courrentState = MovementState.crouching;
        else if (runActive && grounded)
            courrentState = MovementState.running;
        else if (grounded)
            courrentState = MovementState.walking;
        else
            courrentState = MovementState.air;
    }
    
    private void Movement()
    {
        float targetSpeed = walkSpeed;

        if (runActive) targetSpeed *= runMultiplier;
        if (crouchActive) targetSpeed *= crouchMultiplier;

        Vector3 desiredVelocity = moveDirection.normalized * targetSpeed;
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 velocityChange = desiredVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void HandleRotation()
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;

        // copiamos solo el giro horizontal de la cámara
        if (camForward.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            Quaternion smoothRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothRotation);
        }
    }

    private void HandleDrag()
    {
        // durante el dash el drag es menor 
        if (dashActive)
        { 
            rb.linearDamping = 0.5f; 
        }
        else if (grounded)
        { 
            rb.linearDamping = groundDrag; 
        }
        else // en el aire
        { 
            rb.linearDamping = 0; 
        }
    }

    private void StartDash()
    {
        // evita spamear dash mientras ya estás en uno o en cooldown
        if (dashActive || !readyToDash) return;

        readyToDash = false;
        dashActive = true;
        dashTimer = maxDashTime;

        // calculamos dirección en base al forward actual del player
        Vector3 inputDir = transform.forward * moveInput.y + transform.right * moveInput.x;
        inputDir.y = 0f;

        // si hay input -> dash en esa dirección
        if (inputDir.sqrMagnitude > 0.01f)
        {
            dashDirection = inputDir.normalized;
        }
        else
        {
            // si NO hay input -> dash hacia atrás
            dashDirection = -transform.forward;
            dashDirection.y = 0f;
            dashDirection.Normalize();
        }

        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void DashMovement()
    {
        // igual que tu antiguo slide: empuje durante unos frames
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
    }

    private void HandleDashTimer()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            StopDash();
        }
    }

    private void StopDash()
    {
        dashActive = false;
    }
    
    private void ResetDash()
    {
        readyToDash = true;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    
        float maxSpeed = walkSpeed;

        // durante dash usamos otro límite para no caparlo raro
        if (dashActive)
        {
            maxSpeed = dashForce;
        }
        else
        {
            if (runActive) maxSpeed *= runMultiplier;
            if (crouchActive) maxSpeed *= crouchMultiplier;
        }

        if(flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    public void DoubleJump()
    {
        if (doubleJumpsLeft <= 0) return;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        doubleJumpsLeft--;
    }
    
    private void ResetJump()
    {
        readyToJump = true;
    }

    public void ResetDoubleJumps()
    {
        doubleJumpsLeft = doubleJump;
    }
}