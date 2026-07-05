using UnityEngine;

public class SC_JumpComponent : MonoBehaviour
{
    private SC_MasterCharacterMovement master;
    private SC_PlayerManaBar mana;
    private Animator anim;
    
    [SerializeField] private SC_ScriptableEvents yokaiSkill;
    [SerializeField] private SC_ScriptableEvents yokaiBatery;
    [SerializeField] private SC_ScriptableFloatEvent eventoMana;
    
    [Header("Jump")]
    [SerializeField] private float manaCost;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private int doubleJump;

    [Header("VFX Settings")]
    [SerializeField] private GameObject dustVFXPrefab;
    
    private bool readyToJump = true;
    private int doubleJumpsLeft;

    private void Awake()
    {
        master = GetComponent<SC_MasterCharacterMovement>();
        mana = Object.FindFirstObjectByType<SC_PlayerManaBar>();
        doubleJumpsLeft = doubleJump;
        anim = GetComponentInChildren<Animator>();
    }
    
    public void HandleJumpInput()
    {
        if (readyToJump && master.Grounded)
        {
            readyToJump = false;
            Jump();
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        else if (!master.Grounded)
        {
            DoubleJump();
        }
    }

    private void Jump()
    {
        // se resetea velocidad vertical para que el salto doble no acumule la velocidad del anterior
        master.Rb.linearVelocity = new Vector3(master.Rb.linearVelocity.x, 0f, master.Rb.linearVelocity.z);
        anim.SetTrigger("Jump");
        
        if (dustVFXPrefab != null)
        {
            Vector3 basePosition = master.Feet.position;
            Vector3 spawnPosition = new Vector3(basePosition.x, basePosition.y + 2f, basePosition.z);
            
            GameObject dustInstance = Instantiate(dustVFXPrefab, spawnPosition, Quaternion.identity);
            Destroy(dustInstance, 0.5f);
        }
        
        master.Rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void DoubleJump()
    {
        if (mana.PlayerActualMana < manaCost)
        {
            yokaiBatery.Raise();
            return;
        }
        if (doubleJumpsLeft <= 0) return;

        Jump();
        eventoMana.Raise(manaCost);
        yokaiSkill.Raise();
        doubleJumpsLeft--;
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    //se llama en el master ;)
    public void ResetDoubleJumpsIfNeeded()
    {
        if (doubleJumpsLeft != doubleJump)
        {
            doubleJumpsLeft = doubleJump;
        }
    }
}
