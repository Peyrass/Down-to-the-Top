using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SC_EnemyHealth : MonoBehaviour, SC_IHittable
{
    public float health = 3;
    public bool invencibility = false;
    public Rigidbody rb;
    public Animator anim;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        rb.freezeRotation = true;
    }
    
    public void Damage(float damage, Transform attacker)
    {
        if(invencibility) return;
        invencibility = true;
        health -= damage;
        anim.SetTrigger("Hit");
        
        //knockback
        Vector3 dir = (transform.position - attacker.position).normalized;
        dir.y = 0.5f;
        rb.AddForce(dir*10f,ForceMode.Impulse);

        
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
        rb.freezeRotation = false;
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
