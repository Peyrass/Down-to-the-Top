using UnityEngine;

namespace Enemies.Punchie
{
    public class SC_EnemyAttack : MonoBehaviour
    {
        [Header("Configuración del Golpe")]
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float attackDamage = 1f;
        [SerializeField] private LayerMask layerPlayer;

        [Header("VFX Settings")]
        [SerializeField] private GameObject punchImpactPrefab;
        
        [Header("SFX Settings")]
        [SerializeField] private SC_ScriptableAudioEvents aEvent;
        [SerializeField] private AudioClip clip;
        
        public void AttackEvent()
        {
            Collider[] colliders = Physics.OverlapSphere(attackPoint.position, attackRange, layerPlayer);
            
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out SC_IHittable hit))
                {
                    hit.Damage(attackDamage, transform);
                   if(punchImpactPrefab!=null) Instantiate(punchImpactPrefab, attackPoint.position, attackPoint.rotation);
                    aEvent.Raise(clip);
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
}
