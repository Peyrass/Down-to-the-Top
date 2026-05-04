using System.Collections;
using UnityEngine;

public class SC_PlayerBeenHit : MonoBehaviour, SC_IHittable
{
    [Header("General Values")]
    [SerializeField] private SC_ScriptableFloatEvent healthDown;
    [SerializeField] private Animator anim;
    
    [Header("Feedback")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float invincibilityTime = 0.5f;
    private bool isInvincible = false;
    

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    public void Damage(float damage, Transform attacker)
    {
        
        if (isInvincible) return;
        
        //quitar vida mediante el SO
        healthDown.Raise(damage);
        
        isInvincible = true;

        anim.SetTrigger("Hitted");
        
        // knockback
        Vector3 dir = (transform.position - attacker.position).normalized;
        dir.y = 0.5f;
        rb.AddForce(dir*knockbackForce,ForceMode.Impulse);
        
        StartCoroutine(ResetInvincibility());
    }
    private IEnumerator ResetInvincibility()
    {
        // Esperamos el tiempo definido de invencibilidad
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
        Debug.Log("Yomi vuelve a ser vulnerable");
    }
}