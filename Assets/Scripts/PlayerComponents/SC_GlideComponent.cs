using UnityEngine;

public class SC_GlideComponent : MonoBehaviour
{
    private SC_MasterCharacterMovement master;
    
    [Header("Glide Settings")]
    [Tooltip("La velocidad máxima a la que caerá el jugador mientras planea (debe ser negativa)")]
    [SerializeField] private float glideFallSpeed = -2f; 
    

    // Variable pública para que el Master sepa si estamos planeando 
    public bool IsGliding { get; private set; } //pero con private set (solo de lectura)
    public float GlideFallSpeed => glideFallSpeed;

    private bool isJumpInputHeld;

    private void Awake()
    {
        master = GetComponent<SC_MasterCharacterMovement>();
        IsGliding = false;
    }

    // El Master llamará a esta función cuando pulsemos o soltemos el Espacio
    public void SetGlideInput(bool isHeld)
    {
        isJumpInputHeld = isHeld;
    }

    private void Update()
    {
        // Condiciones: No estar en el suelo, estar cayendo (velocidad Y negativa) y mantener el botón pulsado
        if (!master.Grounded && master.Rb.linearVelocity.y < 0 && isJumpInputHeld)
        {
            IsGliding = true;
        }
        else
        {
            IsGliding = false;
        }
    }
}
