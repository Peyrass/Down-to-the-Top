using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies.Sniper
{
    public class SC_EnemyShoot : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private SC_ScriptableAudioEvents aEvent;
        [SerializeField] private AudioClip clip;
        
        [Header("Shoot Settings")]
        [SerializeField] private Transform shootPoint;
        [SerializeField] private float bulletDamage = 2f;
        [SerializeField] private float attackRange = 100f;
        [SerializeField] private LayerMask layerToHit;

        [Header("Targeting")] 
        private Transform targetPlayer;
        [SerializeField] private float aimHeightOffset = 1f; // asegurar que se apunta al centro estimado del player
        
        [Header("Humanization")]
        [Tooltip("Ángulo máximo de desviación de la bala en grados. 0 = Láser perfecto, 2 = Francotirador hábil, 5 = Novato.")]
        [SerializeField] private float maxSpreadAngle = 2.0f;
        [Tooltip("Si está activo, el error aumenta según se alarga la distancia al objetivo.")]
        [SerializeField] private bool distanceAffectsAccuracy = true;

        [Header("VFX Settings")]
        [SerializeField] private GameObject rifleFlashPrefab;
        [SerializeField] private GameObject bulletImpactPrefab;

        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                targetPlayer = player.transform;
            }
        }

        public void ShootEvent()
        {
            aEvent.Raise(clip);
            if (rifleFlashPrefab != null && shootPoint != null)
            {
                Instantiate(rifleFlashPrefab, shootPoint.position, shootPoint.rotation);
            }
            
            Vector3 shootDirection = shootPoint.forward;

            if (targetPlayer != null)
            {
                // se busca el centro del player
                Vector3 targetCenter = targetPlayer.position + Vector3.up * aimHeightOffset;
                // así se consigue la dirección exacta para impactar al player en caso de que esté a la vista
                shootDirection = (targetCenter - shootPoint.position).normalized; 
            }
            
            // disparar rayo
            if (!Physics.Raycast(
                    shootPoint.position,
                    shootDirection,
                    out RaycastHit hitInfo,
                    attackRange,
                    layerToHit)
               ) {Debug.Log("La bala se perdió en el infinitoooo (no chocó con Yomi ni con obstáculos)."); return;}
            
            
            // choca con algo dañable?
            if (hitInfo.collider.TryGetComponent(out SC_IHittable hit))
            {
                Debug.Log("¡La bala alcanzó a Yomi!");
                hit.Damage(bulletDamage, transform);
            }
            else
            {
                Debug.Log("La bala impactó contra un obstáculo: " + hitInfo.collider.name);
            }

            // el VFX se genera siempre, tanto en player como en obstáculo
            if (bulletImpactPrefab != null)
            {
                Instantiate(bulletImpactPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            }
        }

        private void OnDrawGizmos()
        {
            if (shootPoint == null) return;
            Gizmos.color = Color.red;
            
            // Dibuja el láser hacia el jugador si el juego está en Play para que lo veas
            Vector3 drawDirection = shootPoint.forward;
            if (Application.isPlaying && targetPlayer != null)
            {
                Vector3 targetCenter = targetPlayer.position + Vector3.up * aimHeightOffset;
                drawDirection = (targetCenter - shootPoint.position).normalized;
            }
            Gizmos.DrawRay(shootPoint.position, drawDirection * attackRange);
        }
    }
}
