using UnityEngine;

public class SC_CameraManager : MonoBehaviour
{
    [Header("Character References")]
    public SC_MasterCharacterMovement mcm;
    public Transform orientation;
    public Transform player;
    public Transform playerSkin;
    public float rotationSpeed = 7f;

    [Header("Cam Modes")]
    public GameObject explorationCam;
    public GameObject tatakaeCam;

    public CameraStyle currentStyle;
    public enum CameraStyle { Exploration, Tatakae }
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Estado inicial
        SwitchCameraStyle(CameraStyle.Exploration);
    }

    void Update()
    {
        //Cambio de cámara (BETA TESTING)
        //if (mcm.controls.Player.dashInput.WasPerformedThisFrame())
        {
            ToggleCamera();
        }
    }

    private void ToggleCamera()
    {
        if (currentStyle == CameraStyle.Exploration) SwitchCameraStyle(CameraStyle.Tatakae);
        else SwitchCameraStyle(CameraStyle.Exploration);
    }

    public void SwitchCameraStyle(CameraStyle newStyle)
    {
        explorationCam.SetActive(false);
        tatakaeCam.SetActive(false);

        if (newStyle == CameraStyle.Exploration) explorationCam.SetActive(true);
        if (newStyle == CameraStyle.Tatakae) tatakaeCam.SetActive(true);

        currentStyle = newStyle;
    }
}