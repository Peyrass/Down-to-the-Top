using UnityEngine;

public class SC_DashComponent : MonoBehaviour
{
    private SC_MasterCharacterMovement master;

    [Header("Dash")]
    [SerializeField] private float maxDashTime = 0.3f;
    [SerializeField] private float dashForce = 12f; 
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private Animator anim;

    private float dashTimer;
    private bool readyToDash = true;
    private Vector3 dashDirection;

    public bool DashActive { get; private set; }
    public float DashForce => dashForce;

    private void Awake()
    {
        master = GetComponentInParent<SC_MasterCharacterMovement>();
        anim = GetComponentInChildren<Animator>();
    }

    public void StartDash()
    {
        if (DashActive || !readyToDash|| master==null) return;

        readyToDash = false;
        DashActive = true;
        dashTimer = maxDashTime;

        // 1. Se extrae la orientación de la cámara (igual que en el Master)
        Transform cam = master.CameraTransform;
        
        // Si la cámara del master no está asignada, buscamos la principal como plan B
        if (cam == null)
            cam = Camera.main?.transform; 
        
        if (cam == null)
        {
            Debug.LogError("No se encontró una cámara para calcular el Dash.");
            return;
        }

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        //anim.SetTrigger("Dash");
        
        // 2. Calculamos la dirección relativa a la CÁMARA, no al jugador
        Vector3 inputDir = camForward * master.MoveInput.y + camRight * master.MoveInput.x;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            // Dash hacia donde apunta el joystick/teclado
            dashDirection = inputDir.normalized;
        }
        else
        {
            // 3. Si NO hay input: Dash hacia la espalda del personaje (Esquiva)
            // Aquí sí usamos transform.forward porque es una reacción física del cuerpo
            dashDirection = -transform.forward;
        }

        Invoke(nameof(ResetDash), dashCooldown);
    }

    public void DashMovement()
    {
        // empuje continuo mientras dura el dash
        master.Rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
    }

    public void HandleDashTimer()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
            DashActive = false;
    }

    private void ResetDash()
    {
        readyToDash = true;
    }
}