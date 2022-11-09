
using System.Collections;
using WAG.Core.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace WaG
{
    public class StaminaGaugeManager : MonoBehaviour
    {
        [Header("Gauge transparency parameters")] [SerializeField]
        private float fadeSpeed;

        [SerializeField] private float maxAlpha;
        [SerializeField] private float minAlpha;

        private StaminaController stamina;
        private Image backgroundStaminaGauge;
        private Image currentStaminaGauge;
        private float currentAlpha;
        private Color currentStaminaColor;
        private Color backgroundStaminaColor;
        private bool hasFadeOut;
        private bool hasFadeIn;


        private void Awake()
        {
            hasFadeOut = false;
            hasFadeIn = true;
            backgroundStaminaGauge = GetComponent<Image>();
            currentStaminaGauge = transform.GetChild(0).GetComponent<Image>();
            stamina = FindObjectOfType<StaminaController>();
            currentAlpha = maxAlpha;
            backgroundStaminaColor = backgroundStaminaGauge.color;
            currentStaminaColor = currentStaminaGauge.color;
        }

        private void Update()
        {
            currentStaminaGauge.rectTransform.localScale = new Vector3(stamina.Ratio, 1, 1);
            if (stamina.Ratio >= 1)
            {
                if (!hasFadeOut)
                {
                    hasFadeOut = true;
                    hasFadeIn = false;
                    StartCoroutine(FadeOutGauge());
                }
            }

            else if (!hasFadeIn)
            {
                hasFadeIn = true;
                hasFadeOut = false;
                StartCoroutine(FadeInGauge());
            }
        }

        /// <summary>
        /// Fade Out the stamina gauge over a time defined by fadeSpeed
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeOutGauge()
        {
            backgroundStaminaColor = backgroundStaminaGauge.color;
            currentStaminaColor = currentStaminaGauge.color;
            while (backgroundStaminaGauge.color.a > minAlpha && currentStaminaGauge.color.a > minAlpha)
            {
                if (InputManager.Instance.Move != Vector2.zero && InputManager.Instance.Run)
                    break;

                currentAlpha = backgroundStaminaColor.a - (fadeSpeed * Time.deltaTime);
                backgroundStaminaColor.a = currentStaminaColor.a = currentAlpha;
                backgroundStaminaGauge.color = backgroundStaminaColor;
                currentStaminaGauge.color = currentStaminaColor;
                yield return null;
            }
        }

        /// <summary>
        /// Fade In the stamina gauge over a time defined by fadeSpeed
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeInGauge()
        {
            backgroundStaminaColor = backgroundStaminaGauge.color;
            currentStaminaColor = currentStaminaGauge.color;
            while (backgroundStaminaGauge.color.a < maxAlpha && currentStaminaGauge.color.a < maxAlpha)
            {
                currentAlpha = backgroundStaminaColor.a + (fadeSpeed * Time.deltaTime);
                backgroundStaminaColor.a = currentStaminaColor.a = currentAlpha;
                backgroundStaminaGauge.color = backgroundStaminaColor;
                currentStaminaGauge.color = currentStaminaColor;
                yield return null;
            }
        }
    }
}