using UnityEngine;
using UnityEngine.AI;

public class SC_MovementTranslator : MonoBehaviour
{
    private CharacterController cc;
    private NavMeshAgent agent;

    [Header("Knockback Settings")] [SerializeField]
    private float knockbackFriction = 5f;

    [SerializeField] private float gravity = 20f;

    private Vector3 knockbackVelocity;
    private float verticalVelocity;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();

        // el agente calcula la ruta, pero no es el que mueve al bicho directamente
        // el agente solo le rota.
        agent.updatePosition = false;
        agent.updateRotation = true;
    }

    private void Update()
    {
        Vector3 finalMovement = Vector3.zero;

        // gravedad artificial (obligatoria para el CharContr)
        if (!cc.isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        else
        {
            verticalVelocity = -2f;
        }

        finalMovement.y = verticalVelocity;

        if (knockbackVelocity.magnitude > 0.1f)
        {
            finalMovement += knockbackVelocity;
            knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackFriction * Time.deltaTime);
        }
        else
        {
            finalMovement += agent.desiredVelocity;
        }

        cc.Move(finalMovement * Time.deltaTime);
        agent.nextPosition = transform.position;
    }

    public void ApplyKnockback(Vector3 force)
    {
        knockbackVelocity = force;
    }
}
