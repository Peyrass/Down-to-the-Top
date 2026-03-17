using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCharacter : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityScale;
    [SerializeField] private float movementSmoothFactor = 0.3f;
    [SerializeField] private float rotationSmoothFactor = 0.3f;

    [Header("Ground Detection")]
    [SerializeField] private Transform feet;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask whatIsGround;

    private CharacterController controller;
    private Animator anim;
    private Camera cam;

    private bool isGrounded;
    private Vector2 inputVector;
    private Vector3 verticalMovement;
        
    private float currentSpeed;
    private float speedVelocity;
    private float rotationVelocity;

    public PlayerInput PlayerInput { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        PlayerInput = GetComponent<PlayerInput>();
        anim = GetComponentInChildren<Animator>();
        cam = Camera.main;
            
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        PlayerInput.actions["Move"].canceled += UpdateMovement;
        PlayerInput.actions["Move"].performed += UpdateMovement;
        PlayerInput.actions["Jump"].started += Jump;
    }

    private void UpdateMovement(InputAction.CallbackContext ctx)
    {
        inputVector = ctx.ReadValue<Vector2>();
    }
        
    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
        {
            verticalMovement.y = Mathf.Sqrt(-2 * gravityScale * jumpHeight);
        }
    }

    void Update()
    {
        GroundCheck();
        ApplyGravity();
        MoveAndRotate();
    }

    private void MoveAndRotate()
    {
        // Calcular velocidad objetivo (respeta magnitud del joystick)
        float targetSpeed = movementSpeed * inputVector.magnitude;
            
        // Suavizar velocidad actual
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, movementSmoothFactor);
            
        // Calcular y aplicar movimiento
        Vector3 movement = Vector3.zero;
            
        if (inputVector.magnitude > 0)
        {
            // Calcular ángulo de rotación
            float angle = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
                
            // Rotar suavemente
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref rotationVelocity, rotationSmoothFactor);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);//todo
                
            // Mover hacia adelante
            movement = transform.forward * currentSpeed;
        }
            
        // Aplicar movimiento
        controller.Move((movement + verticalMovement) * Time.deltaTime);
            
        // Actualizar animación
        anim.SetFloat("speed", currentSpeed / movementSpeed);
    }

    private void ApplyGravity()
    {
        if (isGrounded && verticalMovement.y < 0)
        {
            verticalMovement.y = -2f;
        }
        else
        {
            verticalMovement.y += gravityScale * Time.deltaTime;
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(feet.position, detectionRadius, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        if (feet != null)
        {
            Gizmos.DrawSphere(feet.position, detectionRadius);
        }
    }
    
}