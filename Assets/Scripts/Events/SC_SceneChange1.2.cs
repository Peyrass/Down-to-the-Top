using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_SceneChange1_2 : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Light exitLight;
    [SerializeField] private Collider exitCollider;

    public void EnableDoor()
    {
        Debug.Log($"Checking enemies: {CheckEnemies()}");

        if (!CheckEnemies())
        {
            Debug.Log("All enemies defeated!");

            exitLight.color = Color.green;
            exitCollider.enabled = true;
        }
    }

    private bool CheckEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy.activeInHierarchy)
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