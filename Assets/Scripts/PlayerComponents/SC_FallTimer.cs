using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SC_FallTimer : MonoBehaviour
{
    [Header("Configuración de Caída")]
    [SerializeField] private float maxAirTime = 4f; // Tiempo que puede estar en el aire antes de morir
    
    private SC_MasterCharacterMovement playerController; 
    
    private Coroutine fallCoroutine;
    private bool isTimerRunning = false;

    private void Awake()
    {
        playerController = GetComponent<SC_MasterCharacterMovement>();
    }

    private void Update()
    {
        if (!playerController.Grounded)
        {
            if (!isTimerRunning)
            { 
                fallCoroutine = StartCoroutine(StartFallCountdown());
            }
        }
        else
        {
            // toco el suelo así que se para todo
            if (!isTimerRunning) return;
            StopCoroutine(fallCoroutine);
            isTimerRunning = false;
            Debug.Log("¡Salvado por los pelos! Temporizador cancelado.");
        }
    }

    private IEnumerator StartFallCountdown()
    {
        isTimerRunning = true;
        Debug.Log($"Yomi ha dejado el suelo. Iniciando cuenta atrás de {maxAirTime} segundos...");

        //espera el tiempo definido
        yield return new WaitForSeconds(maxAirTime);
        
        Debug.Log("¡Tiempo de caída agotado! Reiniciando nivel...");
        RestartLevel();
    }

    private void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
    