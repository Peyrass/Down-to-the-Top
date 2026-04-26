using UnityEngine;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{
    public Image fillBarLife;
    private SC_MasterCharacterMovement playerController;
    private float maxLife;

    void Start()
    {
        playerController = GameObject.Find ("Player").GetComponent <SC_MasterCharacterMovement>();
        maxLife = playerController.life;
    }

    private void Update()
    {
        fillBarLife.fillAmount = playerController.life / maxLife;
    }

}
