using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_MasterCharacterMovement : MonoBehaviour
{
    private TPC_InputMapping controls;
    
    [Header("Movement")]
    private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float airMultiplier;
    //nuevo input system
    private Vector2 moveInput; 
    private Vector3 moveDirection; 
    
    
    [Header("Jumping")]
    bool readyToJump;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private int doubleJump;
    private int doubleJumpsLeft;

    [Header("Crouch & Sliding")] 
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;
    private float regularYScale;
    private float slideTimer;
    private bool slidingActive;
    

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;
    [SerializeField] private Transform orientation;
    
    Rigidbody rb;
    
    
    
    
    
    public MovementState courrentState;
    public enum MovementState
    { walking, running, crouching, sliding, air }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //instanciación de los controles
        controls = new TPC_InputMapping();
    }

    // Suscripciones de eventos
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }


    
    
    private void Start()
    {
        rb.freezeRotation = true;

        readyToJump = true;

        //empieza con la altura normal (2)
        regularYScale = transform.localScale.y;
        
        //tatakaeMode = false;
    }

    private void Update()
    {
        
        moveInput = controls.Player.moveInput.ReadValue<Vector2>();
        
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        // recupera los saltos dobles siempre que se toque el suelo
        if (grounded && doubleJumpsLeft != doubleJump)
        {
            ResetDoubleJumps();
        }
        
        MyInput();
        SpeedControl();
        StateHandler();
        HandleDrag();
        
        if(slidingActive) {HandleSlidingTimer();}
        
    }

    private void FixedUpdate()
    {
        if (slidingActive) {SlidingMovement();}
        else {Movement();}
    }

    private void MyInput()
    {
        //Crouch
        if (controls.Player.crouchInput.WasPerformedThisFrame() && grounded)
        {
            //si se esta moviendo al agacharse entra al estado "Sliding"
            if(moveInput.magnitude >= 0.1f)
            {
                StartSlide();
            }
            //el personaje se encoge
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 1f, ForceMode.Impulse);
            
        }
        
        //Uncrouch
        if (controls.Player.crouchInput.WasReleasedThisFrame())
        {
            transform.localScale = new Vector3(transform.localScale.x, regularYScale, transform.localScale.z);
        }
        
        // Jump
        if(controls.Player.jumpInput.WasPressedThisFrame())
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
    }


    private void StateHandler()
    {
        // Mode - Crouch
        if (slidingActive)
        {
            courrentState = MovementState.sliding;
        }
        
        // Mode - Crouch
        else if (controls.Player.crouchInput.IsPressed())
        {
            courrentState = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        
        // Mode - Running
        else if (grounded && controls.Player.runInput.IsPressed())
        {
            courrentState = MovementState.running;
            moveSpeed = runSpeed;
        }
        
        // Mode - Walking
        else if(grounded)
        {
            courrentState = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        
        //Mode - Air
        else
        {
            courrentState = MovementState.air;
        }
    }
    
    private void Movement()
    {
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        
        //está en el suelo
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        
        //está en el aire
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
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

    private void HandleSlidingTimer()
    {
        slideTimer -= Time.deltaTime;
        if(slideTimer<=0) {StopSlide();}
        
    }
    //Presentación
    private void StartSlide()
    {
        // se activa el bool para mandar la acción al State Handler
        slidingActive = true;
        slideTimer = maxSlideTime;

        // Reducir escala igual que al agacharse
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }
    //Nudo
    private void SlidingMovement()
    {
        Vector3 inputDir = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        // se aplica una fuerza constante durante el slide
        rb.AddForce(inputDir.normalized * slideForce, ForceMode.Impulse);
    }
    //Desenlace
    private void StopSlide()
    {
        slidingActive = false;
        transform.localScale = new Vector3(transform.localScale.x, regularYScale, transform.localScale.z);
    }

    
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limito la velocidad por si acaso
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //calcula (corrobora que la velocidad vertical sea nula)
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        //aplica el salto
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    
    public void DoubleJump()
    {
        if (doubleJumpsLeft <= 0) return;
        
        // get flat velocity
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float flatVelMag = flatVel.magnitude;

        // reinicia la velocidad vertical para que siempre salte a la misma altura
        rb.linearVelocity = orientation.forward * flatVelMag;
        
        // aplica la fuerza
        rb.AddForce(orientation.up * jumpForce, ForceMode.Impulse);

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