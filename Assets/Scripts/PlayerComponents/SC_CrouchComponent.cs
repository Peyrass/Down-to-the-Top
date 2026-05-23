using UnityEngine;

public class SC_CrouchComponent : MonoBehaviour
{
    private Animator anim;

    [Header("Crouch")]
    private float crouchMultiplier = 0.5f;

    public bool CrouchActive { get; private set; }
    public float CrouchMultiplier => crouchMultiplier;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void StartCrouch()
    {
        CrouchActive = true;
        if (anim == null) return;
        anim.SetBool("Crouch", true);
    }

    public void StopCrouch()
    {
        CrouchActive = false;
        anim.SetBool("Crouch", false);
    }
}