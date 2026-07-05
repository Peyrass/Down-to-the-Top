using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class SC_PlayerManaBar : SC_FloatEventListener
{
   [SerializeField] private float playerMaxMana;
   [SerializeField] private float recoverTime;
   [SerializeField] private float recoverAmount;
   private Image manaBar;
   private float playerActualMana;
   private bool manaRecover = false;
   
   public float PlayerActualMana =>playerActualMana;
   public float PlayerMaxMana => playerMaxMana;
   private void Awake()
   {
      manaBar = GetComponent<Image>();
      playerActualMana = playerMaxMana;
      manaBar.fillAmount = playerActualMana / playerMaxMana;
   }
   public override void OnEventRaise(float value)
   {
      playerActualMana -= value;
      if (playerActualMana <= 0) playerActualMana = 0;
      manaBar.fillAmount = playerActualMana / playerMaxMana;

      StopCoroutine(StartRecovering());
      StartCoroutine(StartRecovering());
   }

   private IEnumerator StartRecovering()
   {
      manaRecover = false;
      yield return new WaitForSeconds(recoverTime);
      manaRecover = true;
   }

   private void Update()
   {
      if (manaRecover)
      {
         playerActualMana += recoverAmount * Time.deltaTime;
         manaBar.fillAmount = playerActualMana / playerMaxMana;
         if (playerActualMana >= playerMaxMana) manaRecover = false;
      }
   }
}