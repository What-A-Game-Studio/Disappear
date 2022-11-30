using System.Collections;
using System;
using UnityEngine;
using WAG.Core.Controls;
using WAG.Debugger;
using WAG.Player.Enums;
using WAG.Player.Health;
using WAG.Player.Movements;

namespace WAG.Player
{
    public class PlayerSpeedController : MonoBehaviour
    {
        [Header("Walk")] [SerializeField] private float walkSpeed = 2f;
        [Header("Run")] [SerializeField] private float runSpeedFactor = 0.5f;

        [Header("Weight Modifiers")] [SerializeField]
        private float lightOverweightSpeedModifier = -.2f;
        [SerializeField] private float largeOverweightSpeedModifier = -.5f;
        public Weight PlayerWeight { private get; set; }
        private float teamSpeedModifier = 0;

        private float? temporarySpeedModifier = null;
        private float targetSpeed;
        private StaminaController staminaController;
        private CrouchController crouchController;
        public CrouchController CrouchController => crouchController;
        private PlayerHealthController healthController;
        public PlayerHealthController HealthController => healthController;

        private void GetNeededComponents()
        {
            if (!TryGetComponent<CrouchController>(out crouchController))
            {
                Debug.LogError("Need CrouchController", this);
                Debug.Break();
            }

            if (!TryGetComponent<StaminaController>(out staminaController))
            {
                Debug.LogError("Need StaminaController", this);
                Debug.Break();
            }

            if (!TryGetComponent<PlayerHealthController>(out healthController))
            {
                Debug.LogError("Need playerHealthController", this);
                Debug.Break();
            }
        }

        private void Awake()
        {
            GetNeededComponents();
        }

        public float GetSpeed()
        {
            targetSpeed = walkSpeed;

            if (InputManager.Instance.Move == Vector2.zero)
                return 0f;

            if (InputManager.Instance.Run && (staminaController.CanRun || DebuggerManager.Instance.UnlimitedStamina))
                targetSpeed += targetSpeed * runSpeedFactor;

            walkSpeed *= teamSpeedModifier;

            if (temporarySpeedModifier.HasValue)
                targetSpeed += targetSpeed * temporarySpeedModifier.Value;

            ///TODO : Move this in dedicate componant 
            switch (PlayerWeight)
            {
                case Weight.LightOverweight:
                    targetSpeed += targetSpeed * lightOverweightSpeedModifier;
                    break;
                case Weight.LargeOverweight:
                    targetSpeed += targetSpeed * largeOverweightSpeedModifier;
                    break;
                case Weight.Normal:
                default:
                    break;
            }

            targetSpeed += targetSpeed * crouchController.CrouchSpeedFactor;

            targetSpeed += targetSpeed * HealthController.HealthSpeedModifier;

            targetSpeed *= DebuggerManager.Instance.debugSpeed;
            return targetSpeed;
        }

        private IEnumerator SetTemporarySpeed(float speedModifier, float duration, float? delay = null,
            Action callBack = null)
        {
            //Apply delay
            if (delay.HasValue)
                yield return new WaitForSeconds(delay.Value);

            //Apply speed
            AddSpeedModifier(speedModifier);

            //Reset value
            yield return new WaitForSeconds(duration);
            temporarySpeedModifier -= speedModifier;
            if (temporarySpeedModifier <= 0)
                temporarySpeedModifier = null;

            callBack?.Invoke();
        }

        #region Public methods

        /// <summary>
        /// Set a temporary speed for seconds 
        /// </summary>
        /// <param name="speedModifier">Speed modifier value</param>
        /// <param name="duration">Time in seconds</param>
        /// <param name="delay">Delay speed modifier in seconds</param>
        public void SetTemporarySpeedForSeconds(float speedModifier, float duration, float? delay = null,
            Action callBack = null)
        {
            StartCoroutine(SetTemporarySpeed(speedModifier, duration, delay, callBack));
        }

        public void AddSpeedModifier(float speedModifier)
        {
            if (temporarySpeedModifier.HasValue)
                temporarySpeedModifier += speedModifier;
            else
                temporarySpeedModifier = speedModifier;
        }

        public void RemoveSpeedModifier(float speedModifier)
        {
            if (temporarySpeedModifier.HasValue)
                temporarySpeedModifier -= speedModifier;
        }

        /// <summary>
        /// Set Speed by team
        /// </summary>
        /// <param name="teamDataSpeedModifier"></param>
        public void SetTeamSpeedModifier(float teamDataSpeedModifier)
        {
            teamSpeedModifier = teamDataSpeedModifier;
        }

        #endregion Public methods
    }
}