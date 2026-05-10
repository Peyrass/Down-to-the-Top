using System;
using UnityEngine;
using UnityEngine.UI;

public class SC_PlayerHealthBar : SC_FloatEventListener
{
    [SerializeField] private float playerMaxHealth;
    [SerializeField] private SC_ScriptableEvents onDead;
    private Image healthBar;
    private float playerActualHealth;

    private void Awake()
    {
        healthBar = GetComponent<Image>();
        playerActualHealth = playerMaxHealth;
        healthBar.fillAmount = playerActualHealth / playerMaxHealth;
    }

    public override void OnEventRaise(float value)
    {
        playerActualHealth -= value;

        if (playerActualHealth <= 0)
        {
            playerActualHealth = 0;
            onDead.Raise();
        }
        else if (playerActualHealth >= playerMaxHealth) playerActualHealth = playerMaxHealth;
        
        healthBar.fillAmount = playerActualHealth / playerMaxHealth;
    }
}