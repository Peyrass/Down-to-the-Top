using UnityEngine;

public class SC_KoiProjectileLogic : MonoBehaviour
{
    [SerializeField] private float danhoAmount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SC_PlayerBeenHit si))
        {
                if (other.TryGetComponent(out SC_IHittable danho))
                {
                    danho.Damage(danhoAmount, transform);
                    Destroy(gameObject);
                }
        }

        else if (other.CompareTag("whatIsGround"))
        {
            // Si el proyectil golpea el entorno, también se destruye
            Destroy(gameObject);
        }
    }
}
