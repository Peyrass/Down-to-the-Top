using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SC_ChangeScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}