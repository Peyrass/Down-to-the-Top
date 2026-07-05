using UnityEngine;

public class SC_CatDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SC_PlayerBeenHit si))
        {
            if (other.TryGetComponent(out SC_IHittable damage))
            {
                damage.Damage(damageAmount, transform);
            }
        }
    }
}
