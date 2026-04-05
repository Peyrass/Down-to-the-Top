using UnityEngine;

public class SC_JumpComponent : MonoBehaviour
{
    private SC_MasterCharacterMovement master;

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private int doubleJump;

    private bool readyToJump = true;
    private int doubleJumpsLeft;

    private void Awake()
    {
        master = GetComponent<SC_MasterCharacterMovement>();
        doubleJumpsLeft = doubleJump;
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
        master.Rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void DoubleJump()
    {
        if (doubleJumpsLeft <= 0) return;

        master.Rb.linearVelocity = new Vector3(master.Rb.linearVelocity.x, 0f, master.Rb.linearVelocity.z);
        master.Rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        doubleJumpsLeft--;
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    public void ResetDoubleJumpsIfNeeded()
    {
        if (doubleJumpsLeft != doubleJump)
            doubleJumpsLeft = doubleJump;
    }
}
