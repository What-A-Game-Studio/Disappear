using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WaG
{
    public class StaminaGaugeManager : MonoBehaviour
    {
        private StaminaController stamina;
        private Image backgroundStaminaGauge;
        private Image currentStaminaGauge;
        
        private void Awake()
        {
            backgroundStaminaGauge = GetComponent<Image>();
            currentStaminaGauge = transform.GetChild(0).GetComponent<Image>();
            stamina = FindObjectOfType<StaminaController>();
        }

        private void Update()
        {
            currentStaminaGauge.rectTransform.localScale = new Vector3(stamina.Ratio,1,1);
        }
    }
}
