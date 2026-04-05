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

    //nuevo input system
    public Vector2 moveInput;
    public Vector3 moveDirection;
    
    [Header("Jumping")]
    bool readyToJump;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private int doubleJump;
    private int doubleJumpsLeft;

    [Header("Crouch & Sliding")] 
    [SerializeField] private float crouchMultiplier;
    [SerializeField] private float crouchYScale;
    private bool crouchActive = false;
    
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;
    private float regularYScale;
    private float slideTimer;
    private bool slidingActive;

    [Header("Ground Check")]
    [SerializeField] private Transform feet;
    [SerializeField] private float detectionRadius = 0.3f;
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform orientation;
    private bool grounded;
    
    Rigidbody rb;
    
    public MovementState courrentState;
    public enum MovementState
    { walking, running, crouching, sliding, air }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = GetComponent<PlayerInput>();
    }

    // Suscripciones de eventos
    private void OnEnable()
    {
        controls.actions["moveInput"].performed += MoveAction;
        controls.actions["moveInput"].canceled += MoveAction;
        controls.actions["jumpInput"].started += JumpAction;
        controls.actions["runInput"].started += RunStartedAction;
        controls.actions["runInput"].canceled += RunCanceledAction;
        controls.actions["crouchInput"].started += CrouchStartedAction;
        controls.actions["crouchInput"].canceled += CrouchCanceledAction;
    }

    private void OnDisable()
    {
        controls.actions["moveInput"].performed -= MoveAction;
        controls.actions["moveInput"].canceled -= MoveAction;
    }

    private void MoveAction(InputAction.CallbackContext obj)
    {
        // guardamos el input (NO movemos aquí al personaje)
        moveInput = obj.ReadValue<Vector2>();

        // convertimos el input a dirección relativa a la cámara
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
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
        crouchActive = true;
    }

    private void CrouchCanceledAction(InputAction.CallbackContext obj)
    {
        crouchActive = false;
    }

    private void Start()
    {
        moveSpeed = walkSpeed;
        rb.freezeRotation = true;

        readyToJump = true;

        //empieza con la altura normal
        regularYScale = transform.localScale.y;
    }

    private void Update()
    {
        GroundCheck();
        // recupera los saltos dobles siempre que se toque el suelo
        if (grounded && doubleJumpsLeft != doubleJump)
        {
            ResetDoubleJumps();
        }
        
        MyInput();
        StateHandler();
        HandleDrag();
        
        if(slidingActive) {HandleSlidingTimer();}
    }
    
// TODA la física del Rigidbody va aquí)
    private void FixedUpdate()
    {
        ApplyGravity();
        
        if (slidingActive) 
        { SlidingMovement(); }
        else 
        { Movement(); }
        
        if (!grounded)
        {
            rb.AddForce(Vector3.down * 20f, ForceMode.Force);
        }
        
        SpeedControl();
        
    }
    
    private void GroundCheck()
    {
        grounded = Physics.CheckSphere(feet.position, detectionRadius, whatIsGround);
    }
    
    private void ApplyGravity()
    {
        // si está en el suelo y cayendo → lo “pegamos” al suelo
        if (grounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -2f, rb.linearVelocity.z);
        }
        else
        {
            // aplicamos gravedad extra
            rb.AddForce(Vector3.down * gravityScale, ForceMode.Force);
        }
    }
    private void MyInput()
    {
        //Crouch
        if (crouchActive)
        {
            // el personaje se encoge
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);

            // si se mueve mientras está agachado → slide
            if(moveInput.magnitude >= 0.1f && grounded)
            {
                StartSlide();
            }
        }
        //Uncrouch
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, regularYScale, transform.localScale.z);
        }   
    }

    private void StateHandler()
    {
        if (slidingActive)
            courrentState = MovementState.sliding;
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
        //Se calcula la velocidad objetivo

        float targetSpeed = walkSpeed;

        if (runActive) {targetSpeed *= runMultiplier;}
        if (crouchActive) {targetSpeed *= crouchMultiplier;}

        // velocidad que queremos alcanzar
        Vector3 desiredVelocity = moveDirection.normalized * targetSpeed;

        // velocidad actual del rigidbody
        Vector3 currentVelocity = rb.linearVelocity;

        // diferencia entre los valores para saber cuanta velocidad hay que aplicar
        Vector3 velocityChange = desiredVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);

        // se usa VelocityChange para controlar directamente la velocidad en lugar de acumular fuerzas
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void HandleDrag()
    {
        if (slidingActive)
        { rb.linearDamping = 0.5f; }
        else if (grounded)
        { rb.linearDamping = groundDrag; }
        else
        { rb.linearDamping = 0; }
    }

    //Presentación
    private void StartSlide()
    {
        slidingActive = true;
        slideTimer = maxSlideTime;

        // Reducir escala igual que al agacharse
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);

        // ❗ANTES: aplicabas impulso cada frame → roto
        // ❗AHORA: solo un impulso inicial → sensación de inercia
        rb.AddForce(moveDirection.normalized * slideForce, ForceMode.Impulse);
    }

    //Nudo
    private void SlidingMovement()
    {
        // ya NO metemos fuerza constante → dejamos que la física haga su trabajo
    }

    private void HandleSlidingTimer()
    {
        slideTimer -= Time.deltaTime;
        if(slideTimer<=0) {StopSlide();}
    }

    //Desenlace
    private void StopSlide()
    {
        slidingActive = false;
        transform.localScale = new Vector3(transform.localScale.x, regularYScale, transform.localScale.z);
    }

    private void SpeedControl()
    {
        // limitamos velocidad horizontal
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    
        float maxSpeed = walkSpeed;

        if (runActive) maxSpeed *= runMultiplier;
        if (crouchActive) maxSpeed *= crouchMultiplier;

        // ❗SIN ESTO → velocidad infinita
        if(flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reseteamos velocidad vertical para salto consistente
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // aplica el salto
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    public void DoubleJump()
    {
        if (doubleJumpsLeft <= 0) return;

        // mismo principio → reset vertical
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