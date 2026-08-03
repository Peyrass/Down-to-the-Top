using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.Behavior;

public class SC_EnemyHealth : MonoBehaviour, SC_IHittable
{
    [Header("Presets")]
    private float currentHealth = 0;
    public float maxHealth = 3;
    public bool invencibility = false;
    
    [Header("Componentes")]
    private BehaviorGraphAgent behaviorGraph;
    public Rigidbody rb;
    public Animator anim;
    public NavMeshAgent agent;
    [SerializeField] private float deathTime = 2f;
    
    [Header("BOSS Enemies only")]
    [Tooltip("Si está activo, el error aumenta según se alarga la distancia al objetivo.")]
    [SerializeField] public Transform finalBoss;
    [SerializeField] private SC_SceneChange1_2 changeScene1_2;
    
    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        agent = GetComponentInParent<NavMeshAgent>();
        behaviorGraph = GetComponentInParent<BehaviorGraphAgent>();
        currentHealth = maxHealth;
    }
    
    public void Damage(float damage, Transform attacker)
    {
        if(invencibility) return;
        invencibility = true;
        currentHealth -= damage;
        if (anim != null) anim.SetTrigger("Hitted");
        
        //knockback
        if (behaviorGraph != null) behaviorGraph.enabled = false;
        if (agent != null) agent.enabled = false;
        if (rb != null) rb.isKinematic = false;

        
        Vector3 dir = (transform.position - attacker.position).normalized;
        dir.y = 0.5f;
        if (rb != null) rb.AddForce(dir * 10f, ForceMode.Impulse);
        

        StartCoroutine(currentHealth > 0 ? OnHit() : OnDeath());
    }

    private IEnumerator OnHit()
    {
        yield return new WaitForSeconds(0.5f);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (agent != null)
        {
            agent.Warp(transform.position); 
            agent.enabled = true;
        }
        
        if (behaviorGraph != null) behaviorGraph.enabled = true;
        
        invencibility = false;
    }
    private IEnumerator OnDeath()
    {
        if (agent != null) agent.enabled = false;
        if (rb != null) rb.freezeRotation = false;
        yield return new WaitForSeconds(deathTime);
        if (finalBoss != null)
        {
           SC_BossLogic bossLogic = finalBoss.GetComponent<SC_BossLogic>();
            if (bossLogic != null)
            {
                bossLogic.TakeDamage(maxHealth);
            }
            else
            {
                Debug.LogWarning("¡Cuidado! El transform 'finalBoss' está asignado pero no tiene el script SC_BossLogic.");
            }
        }
        //ternario para comprobar si el script está o no en el parent y así destruír bien al enemigo.
        GameObject objectToDestroy = transform.parent != null ? transform.parent.gameObject : gameObject;

        objectToDestroy.SetActive(false);
        if (changeScene1_2 != null) changeScene1_2.EnableDoor();

        Destroy(objectToDestroy);
    }
}
