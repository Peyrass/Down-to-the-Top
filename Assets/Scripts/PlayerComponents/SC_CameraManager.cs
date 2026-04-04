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
        // Orientación horizontal (Ignora la altura de la cámara para que el personaje no se incline)
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;
        
        HandleRotation();

        // 2. Cambio de cámara (BETA TESTING)
        if (mcm.controls.Player.dashInput.WasPerformedThisFrame())
        {
            ToggleCamera();
        }
    }

    private void HandleRotation()
    {
        // Modo Exploración: el personaje puede rotar libremente en función a los inputs
        if (currentStyle == CameraStyle.Exploration)
        {
            Vector3 inputDir = orientation.forward * mcm.moveInput.y + orientation.right * mcm.moveInput.x;
            if (inputDir != Vector3.zero)
            {
                playerSkin.forward = Vector3.Slerp(playerSkin.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
            }
        }
        else if (currentStyle == CameraStyle.Tatakae)
        {
            // Modo Tatakae: el personaje siempre mira al frente (strafe = On ;) )
            playerSkin.forward = Vector3.Slerp(playerSkin.forward, orientation.forward, Time.deltaTime * rotationSpeed);
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