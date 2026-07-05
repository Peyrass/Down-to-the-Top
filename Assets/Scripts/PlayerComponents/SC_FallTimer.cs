using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PlayerComponents
{
    public class SC_FallTimer : MonoBehaviour
    {
        [Header("Configuración de Caída")]
        [SerializeField] private float maxAirTime = 1.5f; 
    
        [Header("UI del Timer")]
        [SerializeField] private GameObject timerCanvas; 
        [SerializeField] private Image radialBar;
        [SerializeField] private Color safeColor = Color.white;
        [SerializeField] private Color dangerColor = Color.red;
    
        private SC_MasterCharacterMovement playerController; 
        private Coroutine fallCoroutine;
        private bool isTimerRunning = false;
    
        private Camera mainCamera;
    
        private void Awake()
        {
            playerController = GetComponent<SC_MasterCharacterMovement>();
            if (timerCanvas != null) timerCanvas.SetActive(false);
        
            mainCamera = Camera.main;
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
                if (!isTimerRunning) return;
                StopCoroutine(fallCoroutine);
                isTimerRunning = false;
            
                if (timerCanvas != null) timerCanvas.SetActive(false); 
            }
        }
    
        private void LateUpdate()
        {
            if (timerCanvas != null && timerCanvas.activeSelf)
            {
                timerCanvas.transform.forward = mainCamera.transform.forward;
            }
        }

        private IEnumerator StartFallCountdown()
        {
            isTimerRunning = true;
        
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
        
            RestartLevel();
        }

        private void RestartLevel()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.buildIndex);
        }
    }
}