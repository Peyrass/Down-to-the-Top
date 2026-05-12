using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SC_BossLogic : MonoBehaviour
{
    [SerializeField] private List <GameObject> enemyPrefab;
    [SerializeField] private GameObject[] dollVariants;
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] public float bossMaxHealth;
    [SerializeField] public float bossAvailableHealth;

    private void Start()
    {
        bossAvailableHealth = bossMaxHealth;// Initialize the boss's available health to its maximum health at the start of the game
        SpawnEnemy();
        
    }
    private void Update()
    {
        
    }

    public void SpawnEnemy()
    {
        if (bossAvailableHealth <= 0) return; // If the boss has no available health left, do not spawn any more enemies
        Transform spawnPoint = SetSpawnLocation();
        GameObject enemy = Instantiate(SetEnemy());
        enemy.transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y+ enemy.transform.position.y, spawnPoint.position.z) ;
        Invoke("SpawnEnemy", 1);
    }

    private GameObject SetEnemy()
    {
        while (true)
        {
            int randomEnemy = Random.Range(0, enemyPrefab.Count);
            Debug.Log("Random Enemy Index: " + randomEnemy);
            if (enemyPrefab[randomEnemy].GetComponent<SC_EnemyHealth>().health <= bossAvailableHealth)
            {
                if (randomEnemy == 0) // If the randomly selected enemy is the doll variant
                {
                    int randomDollVariant = Random.Range(0, dollVariants.Length);
                    enemyPrefab[randomEnemy] = dollVariants[randomDollVariant]; // Replace the enemy prefab with a random doll variant
                }
                bossAvailableHealth -= enemyPrefab[randomEnemy].GetComponent<SC_EnemyHealth>().health; // Reduce the boss's available health by the enemy's health
                return enemyPrefab[randomEnemy];
            }
            enemyPrefab.RemoveAt(randomEnemy); // Remove the enemy from the list if it cannot be spawned
        }
    }

    private Transform SetSpawnLocation()
    {
        int spawnPoint = Random.Range(0, spawnLocations.Length);
        return spawnLocations[spawnPoint];
    }
}
