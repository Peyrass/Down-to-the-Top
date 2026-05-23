using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SC_EnemyHealth : MonoBehaviour, SC_IHittable
{
    private float currentHealth = 0;
    public float maxHealth = 3;
    public bool invencibility = false;
    public Rigidbody rb;
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
    }
    
    public void Damage(float damage, Transform attacker)
    {
        if(invencibility) return;
        invencibility = true;
        currentHealth -= damage;
        //anim.SetTrigger("Hit");
        
        if (agent != null) agent.enabled = false;
        
        //knockback
        Vector3 dir = (transform.position - attacker.position).normalized;
        dir.y = 0.5f;
        if (rb != null) rb.AddForce(dir * 10f, ForceMode.Impulse);


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
        yield return new WaitForSeconds(deathTime);
        if (finalBoss != null)
        {
            finalBoss.GetComponent<SC_BossLogic>().TakeDamage(maxHealth); 
        }

        gameObject.SetActive(false);
        if (changeScene1_2 != null) changeScene1_2.EnableDoor();

        Destroy(gameObject);

    }

}
