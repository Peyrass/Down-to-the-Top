using UnityEngine;

public class GameRespawn : MonoBehaviour
{
    [SerializeField] private float threshold;
    void FixedUpdate()
    {
        if (transform.position.y < threshold)
        {
            transform.position = new Vector3(1.77f, 1.55f, 0);
        }
    }
}
