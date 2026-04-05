using UnityEngine;

public class SC_CrouchComponent : MonoBehaviour
{
    [Header("Crouch")]
    [SerializeField] private float crouchMultiplier;

    public bool CrouchActive { get; private set; }
    public float CrouchMultiplier => crouchMultiplier;

    public void StartCrouch()
    {
        CrouchActive = true;
    }

    public void StopCrouch()
    {
        CrouchActive = false;
    }
}