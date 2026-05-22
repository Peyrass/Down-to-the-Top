using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class SC_CatLogic : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;

    private NavMeshAgent agent; 
    private GameObject player;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); 
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent.SetDestination(player.transform.position);
        targetToFollow = SC_GameManager.Instance.playerFeet;

    }

    private void Update()
    {
        if (targetToFollow == null || agent == null)
            return;

        if (agent.enabled)
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

    private bool DestinationReached()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }
        
    private void LookToTarget()
    {
        Vector3 direction = targetToFollow.position - transform.position;

        // Solo rotaci�n en Y
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
