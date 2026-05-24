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
    public Animator anim;
    public NavMeshAgent agent;
    [SerializeField] private float deathTime = 2f;
    
    [Header("BOSS Enemies only")]
    [SerializeField] public Transform finalBoss;
    [SerializeField] private SC_SceneChange1_2 changeScene1_2;

    private void Awake()
    {
        // He cambiado GetComponentInParent por GetComponent para asegurar que coge el del propio enemigo
        rb = GetComponent<Rigidbody>(); 
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
    }
    
    public void Damage(float damage, Transform attacker)
    {
        if(invencibility) return;
        invencibility = true;
        currentHealth -= damage;
        
        // 1. Apagamos la inteligencia artificial
        if (agent != null) agent.enabled = false;
        
        // 2. AÑADIDO: Encendemos las físicas (deja de ser cinemático)
        if (rb != null) rb.isKinematic = false; 
        
        // 3. Calculamos la dirección y empujamos
        Vector3 dir = (transform.position - attacker.position).normalized;
        dir.y = 0.5f;
        
        // AÑADIDO: He subido un poco la fuerza a 20f. Si sale volando muy lejos, bájalo a 10f de nuevo.
        if (rb != null) rb.AddForce(dir * 20f, ForceMode.Impulse);

        if (currentHealth > 0)
            StartCoroutine(OnHit());
        else
            StartCoroutine(OnDeath());
    }

    private IEnumerator OnHit()
    {
        yield return new WaitForSeconds(0.5f);
        
        // 4. AÑADIDO: Al terminar el golpe, volvemos a hacer al enemigo inmune a las físicas
        if (rb != null) rb.isKinematic = true; 
        
        // 5. Encendemos la inteligencia artificial de nuevo
        if (agent != null) agent.enabled = true;
        
        invencibility = false;
    }

    private IEnumerator OnDeath()
    {
        if (agent != null) agent.enabled = false;
        
        // Al morir, que las físicas sigan activas para que el cadáver caiga al suelo bien
        if (rb != null) rb.isKinematic = false; 
        
        yield return new WaitForSeconds(deathTime);
        if (finalBoss != null)
        {
            finalBoss.GetComponent<SC_BossLogic>().TakeDamage(maxHealth); 
        }

        if(changeScene1_2 != null) changeScene1_2.EnableDoor();

        Destroy(gameObject);
    }
}
