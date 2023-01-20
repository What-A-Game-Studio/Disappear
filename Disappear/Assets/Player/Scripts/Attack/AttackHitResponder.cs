using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using WAG.Core.Controls;
using WAG.HitHurtBoxes;
using WAG.HitHurtBoxes.Class;
using WAG.Player.Health;

namespace WAG.Player.Attacks
{
    public class AttackHitResponder : CompHitResponder
    {
        public static readonly int AttackSpeedModifierHash = "Grounded".GetHashCode();
        
        
        [Header("Values")] [SerializeField] private float attackMissCooldown = 2f;
        [SerializeField] private float attackMissSpeedModifier = -0.8f;
        [SerializeField] private float attackHitCooldown = 2.6f;

        [SerializeField] private float attackHitSpeedModifier = -0.9f;

        // [SerializeField] private float chargeTime = 0.3f; //Direct in InputManager
        [SerializeField] private float chargedAttackSpeedModifier = 0.5f;

        private bool canAttack = true;

        private NGOPlayerSpeedController speedController;

        protected override void Awake()
        {
            base.Awake();
            if (!transform.parent.TryGetComponent<NGOPlayerSpeedController>(out speedController))
            {
                Debug.LogError("AttackHitResponder need to be chiled to PlayerController", this);
                Debug.Break();
            }

            InputManager.Instance.AddCallbackAction(
                ActionsControls.Catch,
                started: context =>
                {
                    if (context.interaction is HoldInteraction)
                    {
                        Debug.Log("started: HoldInteraction");
                        speedController.AddSpeedModifier(AttackSpeedModifierHash,chargedAttackSpeedModifier);
                    }

                    if (context.interaction is PressInteraction)
                        Debug.Log("started: PressInteraction");
                    return;
                },
                performed: context =>
                {
                    if (context.interaction is HoldInteraction)
                    {
                        Debug.Log("performed: HoldInteraction");
                    }

                    if (context.interaction is PressInteraction)
                    {
                        Debug.Log("performed: PressInteraction");
                    }

                    if (!canAttack)
                        return;
                    canAttack = false;

                    Attack();
                },
                canceled: context =>
                {
                    if (context.interaction is HoldInteraction)
                    {
                        Debug.Log("canceled: HoldInteraction");
                        speedController.RemoveSpeedModifier(AttackSpeedModifierHash);
                    }

                    if (context.interaction is PressInteraction)
                        Debug.Log("canceled: PressInteraction");
                });
        }

        private void Attack()
        {
            if (hitBox.CheckHit(out HitData data))
            {
                Debug.Log("CheckHit = true");
                if (data.HurtBox.Owner.parent.TryGetComponent<NGOPlayerHealthController>(
                        out NGOPlayerHealthController phc))
                {
                    Debug.Log("PlayerHealthController = true");
                    phc.TakeDamage();
                    speedController.SetTemporarySpeedForSeconds(
                        hash:AttackSpeedModifierHash,
                        speedModifier: attackHitSpeedModifier,
                        duration: attackHitCooldown,
                        callBack: () =>
                        {
                            Debug.Log("attack hit callback = true");
                            canAttack = true;
                        });
                    return;
                }
            }

            speedController.SetTemporarySpeedForSeconds(
                hash:AttackSpeedModifierHash,
                speedModifier: attackMissSpeedModifier,
                duration: attackMissCooldown,
                callBack: () =>
                {
                    Debug.Log("attack miss callback = true");
                    canAttack = true;
                });
        }
    }
}