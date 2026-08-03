//using UnityEditor.Build; SUPONGO QUE ESTO ES UN ERROR
using UnityEngine;

public class SC_KoiFish : MonoBehaviour
{
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private SC_ScriptableAudioEvents aEvent;
    [SerializeField] private AudioClip clip;

    private void Start()
    {
        Invoke("ShootProjectile", 4f); // Dispara un proyectil cada 3 segundos, ajusta el tiempo segun sea necesario

    }
    private void Update()
    {
        SetKoiPosition();
    }

    public void ShootProjectile()
    {
        aEvent.Raise(clip);
        GameObject projectil = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        projectil.transform.LookAt(SC_GameManager.Instance.player.position); // Asegurate de que el proyectil mire hacia el objetivo
        projectil.GetComponent<Rigidbody>().linearVelocity = projectil.transform.forward * 10f; // Ajusta la velocidad del proyectil seg�n sea necesario
        Invoke("ShootProjectile", 4f); // Dispara un proyectil cada 3 segundos, ajusta el tiempo segun sea necesario
    }

    public void SetKoiPosition()
    {
        Vector3 targetPosition = SC_GameManager.Instance.playerFeet.position;

        // Mantener la misma altura para rotar solo en Y
        targetPosition.y = transform.position.y;

        transform.LookAt(targetPosition);
        transform.Rotate(-90f, 0, -90f); 
    }

}
