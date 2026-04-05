using UnityEngine;

public class SC_DashComponent : MonoBehaviour
{
    private SC_MasterCharacterMovement master;

    [Header("Dash")]
    [SerializeField] private float maxDashTime = 0.3f;
    [SerializeField] private float dashForce = 12f; 
    [SerializeField] private float dashCooldown = 1f;

    private float dashTimer;
    private bool readyToDash = true;
    private Vector3 dashDirection;

    public bool DashActive { get; private set; }
    public float DashForce => dashForce;

    private void Awake()
    {
        master = GetComponent<SC_MasterCharacterMovement>();
    }

    public void StartDash()
    {
        // evita spam y cooldown
        if (DashActive || !readyToDash) return;

        readyToDash = false;
        DashActive = true;
        dashTimer = maxDashTime;

        // dirección en base al input actual
        Vector3 inputDir = transform.forward * master.MoveInput.y + transform.right * master.MoveInput.x;
        inputDir.y = 0f;

        if (inputDir.sqrMagnitude > 0.01f)
            dashDirection = inputDir.normalized;
        else
            dashDirection = -transform.forward;

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