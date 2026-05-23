using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SC_PEY_EnemiesHealth : MonoBehaviour, SC_IHittable
{
    private float currentHealth = 0;
    public float maxHealth = 3;
    public bool invencibility = false;
    
    public Rigidbody rb;
    private SC_MovementTranslator translator;
    
    public Animator anim;
    public NavMeshAgent agent;
    
    [SerializeField] private float deathTime = 2f;
    [SerializeField] public Transform finalBoss;
    [SerializeField] private SC_SceneChange1_2 changeScene1_2;


    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        translator = GetComponent<SC_MovementTranslator>();
    }
    
    public void Damage(float damage, Transform attacker)
    {
        if (invencibility) return;
        invencibility = true;
        currentHealth -= damage;
        
        Vector3 dir = (transform.position - attacker.position).normalized;

        if (translator != null)
        {
            dir.y = 0f;
            translator.ApplyKnockback(dir * 15f);
        }
        else
        {
            if (agent != null) agent.enabled = false;
            
            dir.y = 0.5f;
            if (rb != null) rb.AddForce(dir * 10f, ForceMode.Impulse);
        }

        if (currentHealth > 0)
            StartCoroutine(OnHit());
        else
            StartCoroutine(OnDeath());
    }

    private IEnumerator OnHit()
    {
        yield return new WaitForSeconds(0.5f);
        invencibility = false;
    }
    
    private IEnumerator OnDeath()
    {
        if (agent != null) agent.enabled = false;
        if (translator != null) translator.enabled = false;
        
        yield return new WaitForSeconds(deathTime);
        
        if (finalBoss != null)
        {
            finalBoss.GetComponent<SC_BossLogic>().TakeDamage(maxHealth);
        }

        if (changeScene1_2 != null) changeScene1_2.EnableDoor();

        Destroy(gameObject);
    }

}
