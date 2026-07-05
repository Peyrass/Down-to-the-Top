using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SC_ChangeScene : MonoBehaviour
{

    public static SC_ChangeScene instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}