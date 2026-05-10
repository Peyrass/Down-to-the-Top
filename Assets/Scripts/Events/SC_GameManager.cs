using UnityEngine;

public class SC_GameManager : MonoBehaviour
{
    public static SC_GameManager Instance;
    public Transform playerFeet; 
    public Transform player; 

    private void Awake()
    {
        Instance = this;
    }

}
