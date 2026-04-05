using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Transform[] MainMenu;
    public TMP_Text text;

    void Awake()
    {
        SetTimeScale(0);
    }

    public void SetCursor(bool enabled)
    {
        if (enabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void SetTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;

    }

    public void SetMenu(int menuIndex)
    {
        for (int i = 0; i < MainMenu.Length; i++)
        {
            if (i == menuIndex)
            {
                MainMenu[i].gameObject.SetActive(true);
            }
            else
            {
                MainMenu[i].gameObject.SetActive(false);
            }
        }
    }

    void Start()
    {
        SetCursor(true);
        SetMenu(0);
    }

    public void LoadGameScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void StartButon()
    {
        Time.timeScale = 1;
        SetMenu(3);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
