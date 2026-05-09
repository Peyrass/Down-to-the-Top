using UnityEngine;

public class SC_KoiProjectileLogic : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Aquí puedes agregar lógica para dañar al jugador o cualquier otra interacción que desees
            Debug.Log("¡El proyectil ha golpeado al jugador!");
            Destroy(gameObject); // Destruye el proyectil después de colisionar con el jugador
        }
        else if (other.CompareTag("whatIsGround"))
        {
            // Si el proyectil golpea el entorno, también se destruye
            Destroy(gameObject);
        }
    }
}
