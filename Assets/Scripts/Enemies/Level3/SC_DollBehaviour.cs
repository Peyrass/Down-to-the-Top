using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SC_DollBehaviour : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [SerializeField] private Transform targetToFollow;

    [Header("Attack Behavior")]
    [SerializeField] private float explosionRadius;
    [SerializeField] private LayerMask whatIsDamagable;

    [SerializeField] private int secondsToExplosion = 4;

    private float leftTimeToExplosion;
    private bool isExploding;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Asignar objetivo desde el GameManager
        if (SC_GameManager.Instance != null &&
            SC_GameManager.Instance.playerFeet != null)
        {
            targetToFollow = SC_GameManager.Instance.playerFeet;
        }
        else
        {
            Debug.LogError("GameManager o playerFeet no están asignados");
        }
    }

    private void Update()
    {
        // Seguridad extra
        if (targetToFollow == null || agent == null)
            return;

        if (agent.enabled && !isExploding)
        {
            agent.SetDestination(targetToFollow.position);

            if (DestinationReached())
            {
                LookToTarget();

                if (animator != null)
                {
                    animator.SetBool("Reached", true);
                }

                agent.isStopped = true;
            }
        }
    }

    private void LookToTarget()
    {
        Vector3 direction = targetToFollow.position - transform.position;

        // Solo rotación en Y
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private bool DestinationReached()
    {
        return !agent.pathPending &&
               agent.remainingDistance <= agent.stoppingDistance;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !isExploding)
        {
            StartExplosion(secondsToExplosion);
        }
    }

    private void StartExplosion(int sec)
    {
        isExploding = true;

        leftTimeToExplosion = sec;

        Debug.Log("Explosion started!");

        StartCoroutine(ExplosionCountdown());
    }

    private IEnumerator ExplosionCountdown()
    {
        while (leftTimeToExplosion > 0)
        {
            leftTimeToExplosion -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("BOOM!");

        // Detectar objetos dañables
        Collider[] hitObjects = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            whatIsDamagable
        );

        foreach (Collider hit in hitObjects)
        {
            Debug.Log("Dañado: " + hit.name);

            // Aquí puedes aplicar daño
            // hit.GetComponent<Vida>()?.TakeDamage();
        }

        Destroy(gameObject);
    }

    // Dibujar radio de explosión en la escena
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}