using PlayerComponents;
using UnityEngine;
using UnityEngine.InputSystem;

//ESTE SCRIPT CONTIENE LAS REFERENCIAS PRINCIPALES DEL PLAYER MANEJA EL MOVIMIENTO BÁSICO DEL PLAYER

public class SC_MasterCharacterMovement : MonoBehaviour
{
    private PlayerInput controls;
    private Rigidbody rb;
    private Animator anim;

    // referencias a los componentes (cada uno se encarga de lo suyo)
    private SC_CrouchComponent crouchComponent;
    private SC_DashComponent dashComponent;
    private SC_JumpComponent jumpComponent;
    private SC_CameraComponent cameraComponent;
    private SC_GlideComponent glideComponent;
    
    [Header("Movement")]
    private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runMultiplier; 
    private bool runActive = false;
    
    [SerializeField] private float gravityScale = 20f;
    [SerializeField] private float groundDrag;
    
    public Vector2 moveInput;
    public Vector3 moveDirection;

    public EMovementState courrentState;
    
    [Header("Ground Check")]
    [SerializeField] private Transform feet;
    [SerializeField] private float detectionRadius = 0.3f;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;
    
    [Header("Camera")]
    
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private Transform cameraTransform;

    public int life = 5; 


    public enum EMovementState
    {
        Walking   = 0,
        Running   = 1,
        Crouching = 2, 
        dashing   = 3,
        air       = 4
    }

    // getters para que los otros componentes puedan manejar info sin romper todo
    public Rigidbody Rb => rb;
    public bool Grounded => grounded;
    public Vector2 MoveInput => moveInput;
    public Transform CameraTransform => cameraTransform;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        controls = GetComponentInChildren<PlayerInput>();

        // se instancian los otros componentes del player
        crouchComponent = GetComponent<SC_CrouchComponent>();
        dashComponent = GetComponent<SC_DashComponent>();
        jumpComponent = GetComponent<SC_JumpComponent>();
        glideComponent = GetComponent<SC_GlideComponent>();
    }

    private void OnEnable()
    {
        controls.actions["moveInput"].performed += MoveAction;
        controls.actions["moveInput"].canceled += MoveAction;
        controls.actions["jumpInput"].started += JumpAction;
        controls.actions["jumpInput"].canceled += JumpCanceledAction;
        controls.actions["runInput"].started += RunStartedAction;
        controls.actions["runInput"].canceled += RunCanceledAction;
        controls.actions["crouchInput"].started += CrouchStartedAction;
        controls.actions["crouchInput"].canceled += CrouchCanceledAction;
        controls.actions["dashInput"].started += DashStartedAction;
    }

    private void OnDisable()
    {
        controls.actions["moveInput"].performed -= MoveAction;
        controls.actions["moveInput"].canceled -= MoveAction;
        controls.actions["jumpInput"].started -= JumpAction;
        controls.actions["jumpInput"].canceled -= JumpCanceledAction;
        controls.actions["runInput"].started -= RunStartedAction;
        controls.actions["runInput"].canceled -= RunCanceledAction;
        controls.actions["crouchInput"].started -= CrouchStartedAction;
        controls.actions["crouchInput"].canceled -= CrouchCanceledAction;
        controls.actions["dashInput"].started -= DashStartedAction;
    }


    private void MoveAction(InputAction.CallbackContext obj)
    {
        // solo guarda el input, la dirección se recalcula continuamente
        moveInput = obj.ReadValue<Vector2>();
    }

    private void JumpAction(InputAction.CallbackContext obj)
    {
        jumpComponent.HandleJumpInput();
        if (glideComponent == null) return;
        glideComponent.SetGlideInput(true);
    }
    private void JumpCanceledAction(InputAction.CallbackContext obj)
    {
        if (glideComponent == null) return;
        glideComponent.SetGlideInput(false);
        
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
        if (!grounded) return;
        crouchComponent.StartCrouch();
    }

    private void CrouchCanceledAction(InputAction.CallbackContext obj)
    {
        crouchComponent.StopCrouch();
    }
    
    private void DashStartedAction(InputAction.CallbackContext obj)
    {
        dashComponent.StartDash();
    }

    //debuggin
    private void OnDrawGizmosSelected()
    {
        if (feet == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(feet.position, detectionRadius);
    }
    
    private void Start()
    {
        // bloquea cursor y lo oculta
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        moveSpeed = walkSpeed;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        GroundCheck();

        // tocando suelo reseteamos doble salto
        if (grounded)
        {
            jumpComponent.ResetDoubleJumpsIfNeeded();
        }

        StateHandler();
        HandleDrag();
        UpdateAnimator();
        // control del tiempo del dash
        if (dashComponent.DashActive)
        {
            dashComponent.HandleDashTimer();
        }
    }

    private void UpdateAnimator()
    {
        //da como resultado 0 o 1
        float speedValue = moveInput.magnitude;

        //así se limíta para que no pase de 1
        speedValue = Mathf.Clamp01(speedValue);

        if (speedValue > 0.01f && !runActive)
        {
            speedValue *= 0.5f;
        }
        
        //se para el nombre del parámetro al animator suavizado
        anim.SetFloat("Speed", speedValue, 0.1f, Time.deltaTime);
        
        anim.SetBool("isGrounded", grounded);
        
        anim.SetBool("isFalling", this);
    }

    private void FixedUpdate()
    {
        ApplyGravity();

// 1. calcular dirección
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveDirection = camForward * moveInput.y + camRight * moveInput.x;

// 2. rotar con esa dirección
        HandleRotation();

// 3. mover
        if (dashComponent.DashActive)
        {
            dashComponent.DashMovement();
        }
        else
        {
            Movement();
        }

        SpeedControl();
    }

    private void GroundCheck()
    {
        grounded = Physics.CheckSphere(feet.position, 
            detectionRadius,
            whatIsGround
       );
    }

    private void ApplyGravity()
    {
        if (grounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -2f, rb.linearVelocity.z);
        }
        else if (glideComponent != null && glideComponent.IsGliding)
        {
            // al planear se anulamos la gravedad y la velocidad de caída se reduce
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, glideComponent.GlideFallSpeed, rb.linearVelocity.z);
        }
        else 
        {
            rb.AddForce(Vector3.down * gravityScale, ForceMode.Force);
        }
    }

    private void StateHandler()
    {//switch mejor
        if (dashComponent.DashActive)
            courrentState = EMovementState.dashing;
        else if (crouchComponent.CrouchActive)
            courrentState = EMovementState.Crouching;
        else if (runActive && grounded)
            courrentState = EMovementState.Running;
        else if (grounded)
            courrentState = EMovementState.Walking;
        else
            courrentState = EMovementState.air;
    }
    
    private void Movement()
    {
        float targetSpeed = walkSpeed;

        if (runActive) 
            targetSpeed *= runMultiplier;
        if (crouchComponent.CrouchActive) 
            targetSpeed *= crouchComponent.CrouchMultiplier;

        Vector3 desiredVelocity = moveDirection.normalized * targetSpeed;
        Vector3 currentVelocity = rb.linearVelocity;

        // cuánto hay que ajustar velocidad
        Vector3 velocityChange = desiredVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void HandleRotation()
    {
        if (moveDirection.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        Quaternion smoothRotation = Quaternion.Slerp(
            rb.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(smoothRotation);
    }

    private void HandleDrag()
    {
        // menos drag en dash para que deslice más
        if (dashComponent.DashActive)
            rb.linearDamping = 0.5f;
        else if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0f;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    
        float maxSpeed = walkSpeed;

        if (dashComponent.DashActive)
        {
            maxSpeed = dashComponent.DashForce;
        }
        else
        {
            if (runActive)
                maxSpeed *= runMultiplier;
            if (crouchComponent.CrouchActive)
                maxSpeed *= crouchComponent.CrouchMultiplier;
        }

        if(flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(
                limitedVel.x,
                rb.linearVelocity.y,
                limitedVel.z
                );
        }
    }
}