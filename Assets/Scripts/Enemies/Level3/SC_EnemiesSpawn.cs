using System.Collections.Generic;
using UnityEngine;

public class SC_EnemySpawner : MonoBehaviour
{
    [Header("Doll Prefabs")]
    [SerializeField] private GameObject[] dollPrefabs;

    [Header("Other Enemies")]
    [SerializeField] private GameObject axePrefab;
    [SerializeField] private GameObject turretPrefab;
    [SerializeField] private GameObject catPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] dollSpawnPoints;
    [SerializeField] private Transform[] turretSpawnPoints;
    [SerializeField] private Transform[] catSpawnPoints;

    [Header("Axes Settings")]
    [SerializeField] private Transform axeStartPoint;
    [SerializeField] private float axeSpacing = 2f;

    private void Start()
    {
        SpawnWave();
    }

    public void SpawnWave()
    {
        SpawnDolls();
        SpawnAxes();
        SpawnTurrets();
        SpawnCat();
    }

    // =========================
    // DOLLS
    // =========================

    private void SpawnDolls()
    {
        // Copia temporal para evitar repetir posiciones
        List<Transform> availablePoints = new List<Transform>(dollSpawnPoints);

        foreach (GameObject dollPrefab in dollPrefabs)
        {
            if (availablePoints.Count <= 0)
            {
                Debug.LogWarning("No hay suficientes spawn points para las muñecas");
                return;
            }

            int randomIndex = UnityEngine.Random.Range(0, availablePoints.Count);

            Transform selectedPoint = availablePoints[randomIndex];

            Instantiate(
                dollPrefab,
                selectedPoint.position,
                selectedPoint.rotation
            );

            // Eliminar punto usado
            availablePoints.RemoveAt(randomIndex);
        }
    }

    // =========================
    // AXES
    // =========================

    private void SpawnAxes()
    {
        if (axePrefab == null || axeStartPoint == null)
        {
            Debug.LogWarning("Falta asignar el prefab del hacha o el AxeStartPoint");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 spawnPosition =
                axeStartPoint.position +
                new Vector3(i * axeSpacing, 0f, 0f);

            Instantiate(
                axePrefab,
                spawnPosition,
                axeStartPoint.rotation
            );
        }
    }

    // =========================
    // TURRETS
    // =========================

    private void SpawnTurrets()
    {
        List<Transform> availablePoints = new List<Transform>(turretSpawnPoints);

        for (int i = 0; i < 2; i++)
        {
            if (availablePoints.Count <= 0)
            {
                Debug.LogWarning("No hay suficientes spawn points para las torretas");
                return;
            }

            int randomIndex = UnityEngine.Random.Range(0, availablePoints.Count);

            Transform selectedPoint = availablePoints[randomIndex];

            Instantiate(
                turretPrefab,
                selectedPoint.position,
                selectedPoint.rotation
            );

            // Eliminar punto usado
            availablePoints.RemoveAt(randomIndex);
        }
    }

    // =========================
    // CAT
    // =========================

    private void SpawnCat()
    {
        if (catSpawnPoints.Length <= 0)
        {
            Debug.LogWarning("No hay spawn points para el gato");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, catSpawnPoints.Length);

        Instantiate(
            catPrefab,
            catSpawnPoints[randomIndex].position,
            catSpawnPoints[randomIndex].rotation
        );
    }
}
