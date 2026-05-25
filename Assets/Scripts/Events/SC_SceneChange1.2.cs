using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_SceneChange1_2 : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Light exitLight;
    [SerializeField] private Collider exitCollider;
    
    [SerializeField] private SC_ScriptableAudioEvents aEvent;
    [SerializeField] private AudioClip clip;
    [SerializeField] private SC_ScriptableAudioEvents mEvent;
    [SerializeField] private AudioClip music;

    public void EnableDoor()
    {
        Debug.Log($"Checking enemies: {CheckEnemies()}");

        if (!CheckEnemies())
        {
            Debug.Log("All enemies defeated!");
            aEvent.Raise(clip);
            mEvent.Raise(music);
            
            exitLight.color = Color.green;
            exitCollider.enabled = true;
        }
    }

    private bool CheckEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
            {
                return true;
            }
        }
        // tanto si todos son null como sí están desactivados, devuelve false (abre)
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