using UnityEngine;

public class SC_PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject uiMenu;
    private bool isPaused = false;

    public void Pause()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        uiMenu.SetActive(!uiMenu.activeSelf);
        Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
        isPaused = !isPaused;

        Cursor.visible = !Cursor.visible;
        Cursor.lockState = pauseMenu.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
    
}
