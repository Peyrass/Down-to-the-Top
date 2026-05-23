using UnityEngine;

public class SC_DashComponent : MonoBehaviour
{
    private SC_MasterCharacterMovement master;
    private SC_PlayerManaBar mana;
    
    [SerializeField] private SC_ScriptableEvents eventoYokai;
    [SerializeField] private SC_ScriptableFloatEvent eventoMana;


    [Header("Dash")]
    [SerializeField] private float manaCost;
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
        master = GetComponent<SC_MasterCharacterMovement>();
        mana = Object.FindAnyObjectByType<SC_PlayerManaBar>();
        anim = GetComponentInChildren<Animator>();
    }

    public void StartDash()
    {
        if (mana.PlayerActualMana < manaCost) return;
        if (DashActive || !readyToDash|| master==null) return;

        readyToDash = false;
        DashActive = true;
        dashTimer = maxDashTime;
        eventoMana.Raise(manaCost);
        eventoYokai.Raise();
        anim.SetBool("IsDashing", true);
        
        //Se extrae la orientación de la cámara (igual que en el Master)
        Transform cam = master.CameraTransform;
        
        // se busca la "main camera" por si la cámara del master no está asignada 
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
        
        // Se calcula la dirección relativa a la CÁMARA, no al jugador
        Vector3 inputDir = camForward * master.MoveInput.y + camRight * master.MoveInput.x;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            // dash hacia donde apunta el joystick/teclado
            dashDirection = inputDir.normalized;
        }
        else
        {
            //si no hay input de movimiento el Dash va hacia la espalda del personaje
            dashDirection = -transform.forward;
        }

        Invoke(nameof(ResetDash), dashCooldown);
    }

    public void DashMovement()
    {
        // Empuje continuo mientras dura el dash
        master.Rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
    }

    public void HandleDashTimer()
    {
        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            DashActive = false;
            anim.SetBool("IsDashing", false);
        }
    }

    private void ResetDash()
    {
        readyToDash = true;
    }
}