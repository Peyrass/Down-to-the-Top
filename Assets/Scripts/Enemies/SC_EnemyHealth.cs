using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SC_EnemyHealth : MonoBehaviour, SC_IHittable
{
    public float health = 3;
    public bool invencibility = false;
    public Rigidbody rb;
    public Animator anim;
    public NavMeshAgent agent;
    [SerializeField] private float deathTime = 2f; 


    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    
    public void Damage(float damage, Transform attacker)
    {
        if(invencibility) return;
        invencibility = true;
        health -= damage;
        //anim.SetTrigger("Hit");
        
        if (agent != null) agent.enabled = false;
        
        //knockback
        Vector3 dir = (transform.position - attacker.position).normalized;
        dir.y = 0.5f;
        if (rb != null) rb.AddForce(dir * 10f, ForceMode.Impulse);


        if (health > 0)
            StartCoroutine(OnHit());
        else
            StartCoroutine(OnDeath());
    }


    private IEnumerator OnHit()
    {
        Debug.Log("AU!!  Tengo: " + health + " Hps");
        yield return new WaitForSeconds(0.5f);
        invencibility = false;
    }
    private IEnumerator OnDeath()
    {
        Debug.Log("He muerto x_x");
        
        if (agent != null) agent.enabled = false;
        yield return new WaitForSeconds(deathTime);
        Destroy(gameObject);
    }
}
