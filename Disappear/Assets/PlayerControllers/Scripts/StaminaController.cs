using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaG
{
    public class StaminaController : MonoBehaviour
    {
        public bool CanRun => currentStamina > 0;
        public float Ratio => currentStamina / maxStamina;

        [Header("Stamina Parameters")] [SerializeField]
        private float maxStamina;

        [SerializeField] private float recoveryCooldown;
        [SerializeField] private float recoveryModifier;
        [SerializeField] private float consumptionModifier;
        private float currentRecovery;
        private float currentStamina;
        private bool isRecovering;

        private void Awake()
        {
            currentStamina = maxStamina;
            currentRecovery = recoveryCooldown;
            isRecovering = false;
        }

        private void Update()
        {
            if (InputManager.Instance.Run && InputManager.Instance.Move != Vector2.zero)
            {
                currentStamina -= Time.deltaTime * consumptionModifier;
                if (currentStamina <= 0)
                    isRecovering = true;
            }
            else
            {
                if (isRecovering)
                {
                    currentRecovery -= Time.deltaTime;
                    if (currentRecovery <= 0)
                    {
                        isRecovering = false;
                        currentRecovery = recoveryCooldown;
                    }
                }
                else
                    currentStamina += Time.deltaTime * recoveryModifier;
            }

            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }
}