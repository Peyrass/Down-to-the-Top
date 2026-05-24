using UnityEngine;

public class SC_CrouchComponent : MonoBehaviour
{
    private Animator anim;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchMultiplier = 0.5f;
    [SerializeField] private CapsuleCollider playerCollider;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float groundOffset = 0.1f; // El margen que pedías para no hundirte
    
    private float originalHeight;
    private Vector3 originalCenter;

    public bool CrouchActive { get; private set; }
    public float CrouchMultiplier => crouchMultiplier;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();

        if (playerCollider != null)
        {
            // 1. En el Awake SOLO guardamos cómo era el personaje originalmente para no olvidarlo
            originalHeight = playerCollider.height;
            originalCenter = playerCollider.center;
        }
    }

    public void StartCrouch()
    {
        CrouchActive = true;
        
        if (anim != null) anim.SetBool("Crouch", true);
        
        if (playerCollider != null)
        {
            // 2. Aplicamos la nueva altura
            playerCollider.height = crouchHeight;
            
            // 3. Calculamos la matemática respetando el suelo y el offset, justo en el momento de agacharse
            float bottom = originalCenter.y - (originalHeight / 2f);
            float newCenterY = bottom + (crouchHeight / 2f) + groundOffset;
            
            playerCollider.center = new Vector3(originalCenter.x, newCenterY, originalCenter.z);
        }
    }

    public void StopCrouch()
    {
        CrouchActive = false;
        
        if (anim != null) anim.SetBool("Crouch", false);
        
        if (playerCollider != null)
        {
            // 4. Volvemos a la normalidad usando los datos que guardamos en el Awake
            playerCollider.height = originalHeight;
            playerCollider.center = originalCenter;
        }
    }
}