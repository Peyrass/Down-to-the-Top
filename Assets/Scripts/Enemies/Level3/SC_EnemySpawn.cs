using UnityEngine;

public class SC_EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private Transform[] spawnPoint;

    private void Update()
    {
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        GameObject enemy = SetEnemy();
        enemy.transform.position = spawnPoint[Random.Range(0, spawnPoint.Length)].position;
    }

    private GameObject SetEnemy()
    {
        int randomEnemy = Random.Range(0, enemyPrefab.Length);
        return enemyPrefab[randomEnemy];
    }
}
