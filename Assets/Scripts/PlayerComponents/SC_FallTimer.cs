using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SC_FallTimer : MonoBehaviour
{
    [Header("Configuración de Caída")]
    [SerializeField] private float maxAirTime = 1.5f; // Tiempo que puede estar en el aire antes de morir
    
    [Header("UI del Timer")]
    [SerializeField] private GameObject timerCanvas; 
    [SerializeField] private Image radialBar;
    [SerializeField] private Color safeColor = Color.white;
    [SerializeField] private Color dangerColor = Color.red;
    
    
    private SC_MasterCharacterMovement playerController; 
    
    private Coroutine fallCoroutine;
    private bool isTimerRunning = false;
    
    private void Awake()
    {
        playerController = GetComponent<SC_MasterCharacterMovement>();
        if (timerCanvas != null) timerCanvas.SetActive(false);
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
            
            if (timerCanvas != null) timerCanvas.SetActive(false); 
            Debug.Log("¡Salvado por los pelos! Temporizador cancelado.");
        }
    }

    private IEnumerator StartFallCountdown()
    {
        isTimerRunning = true;
        
        // Aparece la UI al saltar
        if (timerCanvas != null) timerCanvas.SetActive(true);

        float timeElapsed = 0f;
        
        while (timeElapsed < maxAirTime)
        {
            timeElapsed += Time.deltaTime;
            
            if (radialBar != null)
            {
                float fillRatio = 1f - (timeElapsed / maxAirTime);
                radialBar.fillAmount = fillRatio;
                
                radialBar.color = fillRatio < 0.3f ? dangerColor : safeColor;
            }
            
            yield return null; 
        }
        
        Debug.Log("¡Tiempo de caída agotado! Reiniciando nivel...");
        RestartLevel();
    }

    private void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
}
    