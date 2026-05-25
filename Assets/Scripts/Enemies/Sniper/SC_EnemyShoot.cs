using System;
using UnityEngine;

namespace Enemies.Sniper
{
    public class SC_EnemyShoot : MonoBehaviour
    {
        
        
        [Header("Shoot Settings")]
        [SerializeField] private Transform shootPoint;
        [SerializeField] private float bulletDamage = 2f;
        [SerializeField] private float attackRange = 100f;
        [SerializeField] private LayerMask layerToHit;

        [Header("Targeting")]
        private Transform targetPlayer;
        [SerializeField] private float aimHeightOffset = 1f; // asegurar que se apunta al centro estimado del player
        
        [Header("Humanization")]
        [Tooltip("Si está activo, el error aumenta según se alarga la distancia al objetivo.")]
        [SerializeField] private bool distanceAffectsAccuracy = true;
        [Tooltip("Ángulo máximo de desviación de la bala en grados. 0 - 1 = Láser perfecto, 5 = Francotirador hábil, 10+ = Novato.")]
        [SerializeField] private float maxSpreadAngle = 2.0f;

        [Header("VFX Settings")]
        [SerializeField] private GameObject rifleFlashPrefab;
        [SerializeField] private GameObject bulletImpactPrefab;
        
        [Header("SFX Settings")]
        [SerializeField] private SC_ScriptableAudioEvents aEvent;
        [SerializeField] private AudioClip clip;
        
        public void SetTarget(Transform newTarget)
        {
            targetPlayer = newTarget;
        }

        public void ShootEvent()
        {
            aEvent.Raise(clip);
            if (rifleFlashPrefab != null && shootPoint != null)
            {
                Instantiate(rifleFlashPrefab, shootPoint.position, shootPoint.rotation);
            }
            
            Vector3 shootOriginalDirection = shootPoint.forward;
            float distanceToTarget = attackRange;

            if (targetPlayer != null)
            {
                // se busca el centro del player
                Vector3 targetCenter = targetPlayer.position + Vector3.up * aimHeightOffset;
                
                // se saca el vector y la distancia al objetivo
                Vector3 vecToTarget = targetCenter - shootPoint.position;
                distanceToTarget = vecToTarget.magnitude;
                
                //se fija la orientación exacta para impactar a Yomi
                shootOriginalDirection = vecToTarget.normalized;
            }

            float currentSpread = maxSpreadAngle;
            
            if (distanceAffectsAccuracy && targetPlayer != null)
            {
                // 0 = Yomi está frente al Sniper / 1 = Yomi está en el límite del rango de detección
                float distanceFactor = Mathf.InverseLerp(0f, attackRange, distanceToTarget);
                currentSpread = maxSpreadAngle * distanceFactor;
            }

            //se desplaza aleatoriamente la rotación en los ejes X e Y dentro del límite de Spread
            Quaternion randomErrorRotation = Quaternion.Euler(
                UnityEngine.Random.Range(-currentSpread, currentSpread), // Desvío vertical (palo de futbolín)
                UnityEngine.Random.Range(-currentSpread, currentSpread), // Desvío horizontal (barra de pole dance)
                0f
            );
            // Quaternion.LookRotation convierte el vector (shootDirection) en una rotación base a la que se le suma
            // el desvío aleatorio y el resultado se convierte en un vector (shootDirection)
            var shootDirection = Quaternion.LookRotation(shootOriginalDirection) * randomErrorRotation * Vector3.forward;
        
            
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
        
        private void Update()
        {
            if (targetPlayer != null)
            {
                // Dibuja una línea azul en la vista de Escena hacia donde el Sniper cree que está Yomi
                Debug.DrawLine(transform.position, targetPlayer.position, Color.blue);
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
