using UnityEngine;

public class SC_PlayerBeenHit : MonoBehaviour, SC_IHittable
{
    [SerializeField] private SC_ScriptableFloatEvent healthDown;
    public void Damage(float damage, Transform attacker)
    {
        healthDown.Raise(damage);
    }
}
