using System.Collections;
using UnityEngine;

public class SC_PlayerBeenHit : MonoBehaviour, SC_IHittable
{
    [Header("General Values")]
    [SerializeField] private SC_ScriptableFloatEvent healthDown;
    [SerializeField] private SC_ScriptableEvents yokaiVida;
    private SC_PlayerHealthBar health;
    
    [SerializeField] private Animator anim;
    
    [Header("Feedback")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float invincibilityTime = 0.5f;
    private bool isInvincible = false;
    
    
    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        health = Object.FindAnyObjectByType<SC_PlayerHealthBar>();
    }

    public void Damage(float damage, Transform attacker)
    {
        if (isInvincible) return;
        
        //quitar vida mediante el SO
        healthDown.Raise(damage);
        
        isInvincible = true;

        if(anim!=null) anim.SetTrigger("Hitted");
        if (health != null)
        {
            if (health.PlayerActualHealth < health.PlayerMaxHealth * 0.40f)
            {
                yokaiVida.Raise();
            }
        }
        
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