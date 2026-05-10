using UnityEditor.Build;
using UnityEngine;

public class SC_KoiFish : MonoBehaviour
{
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectilePrefab;

    private void Start()
    {
        ShootProjectile();
    }
    private void Update()
    {
        SetKoiPosition();

    }

    public void ShootProjectile()
    {
        GameObject projectil = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        projectil.transform.LookAt(SC_GameManager.Instance.player.position); // Asegúrate de que el proyectil mire hacia el objetivo
        projectil.GetComponent<Rigidbody>().linearVelocity = projectil.transform.forward * 10f; // Ajusta la velocidad del proyectil según sea necesario
    }

    public void SetKoiPosition()
    {
        Vector3 targetPosition = SC_GameManager.Instance.playerFeet.position;

        // Mantener la misma altura para rotar solo en Y
        targetPosition.y = transform.position.y;

        transform.LookAt(targetPosition);
    }

}
