using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_SceneChange1_2 : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Light exitLight;
    [SerializeField] private Collider coll;

    public void EnableDoor()
    { 
        Debug.Log("Checking enemies...");
        if (!CheckEnemies())
        {
            exitLight.color = Color.green;
            coll.enabled = true;
        }
    }

    private bool CheckEnemies()
    {
        Debug.Log("Checking if enemies are still alive...");
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null && enemies[i].activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered the exit trigger.");
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
