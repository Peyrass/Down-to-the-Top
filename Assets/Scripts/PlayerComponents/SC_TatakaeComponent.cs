using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_TatakaeComponent : MonoBehaviour
{
    private PlayerInput controls;
    
    [Header("GeneralValues")]
    [SerializeField] private Animator anim;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float hitStopTime;
    
    [Header("KickValues")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private LayerMask whatIsEnemy;

    [Header("CameraShake")] 
    [SerializeField] private SC_CameraShake camShake;
    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeTime;

    
    private bool isAttacking = false;

    private void Awake()
    {
        controls = GetComponentInChildren<PlayerInput>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        controls.actions["kickInput"].started += AttackAction;
    }

    private void OnDisable()
    {
        controls.actions["kickInput"].started -= AttackAction;
    }

    private void AttackAction(InputAction.CallbackContext obj)
    {
       if(isAttacking) return;
        isAttacking = true;
        anim.SetTrigger("Kick");
        Debug.Log("KIAAA!");
        
        Invoke(nameof(ResetAttack),attackCooldown);
    }

    public void AttackEvent()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, attackRange, whatIsEnemy);
       
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out SC_IHittable hit))
            {
                hit.Damage(attackDamage, transform);
                camShake.ShakeCam(shakeIntensity, shakeTime);
                StartCoroutine(HitStop(0.05f));
            }
        }
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = hitStopTime;
        Debug.Log("HitStop");
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
