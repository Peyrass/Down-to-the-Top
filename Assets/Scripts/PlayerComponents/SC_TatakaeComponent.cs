using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_TatakaeComponent : MonoBehaviour
{
    private PlayerInput controls;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private Animator anim;

    private bool isAttacking = false;

    private void Awake()
    {
        controls = GetComponent<PlayerInput>();
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
        
        Invoke(nameof(ResetAttack),0.2f);
    }

    public void AttackEvent()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, attackRange, whatIsEnemy);
       
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out SC_IHittable hit))
            {
                hit.Damage(attackDamage, transform);
                StartCoroutine(HitStop(0.2f));
            }
        }
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0.1f;
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
