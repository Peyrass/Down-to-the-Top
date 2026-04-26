using UnityEngine;

public class SC_EnemyAttack : MonoBehaviour
{
    [Header("Configuración del Golpe")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private LayerMask layerPlayer;

    
    public void AttackEvent()
    {
        Collider[] colliders = Physics.OverlapSphere(attackPoint.position, attackRange, layerPlayer);
       
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out SC_IHittable hit))
            {
                hit.Damage(attackDamage, transform);
                //aquí también se añadirán los VFX
                Debug.Log("¡Punchie ha golpeado a Yomi!");
            }
        }
    }
    
    //debuggin
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
