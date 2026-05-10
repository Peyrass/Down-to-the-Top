using System;
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

    [SerializeField] int secondsToExplosion = 4;
    private float leftTimeToExplosion;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //targetToFollow = SC_GameManager.Instance.playerFeet; // Asignar el objetivo a seguir desde el GameManager
    }
//nono, esto mejor se hace con Behaviour Graphs
    
    
    private void Update()
    {
        if (agent.enabled)
        {
            agent.SetDestination(targetToFollow.position);

            if (DestinationReached()) //si estas listo y ... PREGUNTA DE EXAMEN
            {
                LookToTarget();
                animator.SetBool("Reached", true);
                agent.isStopped = true; //Me aseguro de estar quieto mientras lanzo el ataque
            }
        }
    }

    private void LookToTarget()
    {
        Vector3 direction = (targetToFollow.position - transform.position);
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction); //le das una direccion y lo transforma en una rotacion
    }
    private bool DestinationReached()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartExplosion(secondsToExplosion);
        }
    }

    private void StartExplosion(int sec)
    {
        leftTimeToExplosion = sec;
        Debug.Log("Explosion started! Time to explosion: ");
        StartCoroutine(ExplosionCountdown());
    }

    IEnumerator ExplosionCountdown()
    {
        while (leftTimeToExplosion > 0)
        {
            leftTimeToExplosion -= Time.deltaTime;
            yield return null;
        }
        Debug.Log("BOOM!");
        // A�adir animaci�n de explosi�n y da�o a jugador
        Destroy(gameObject); 
    }
}
