using Photon.Pun;
using UnityEngine;
using WAG.Core.Controls;
using WAG.HitHurtBoxes;
using WAG.HitHurtBoxes.Class;
using WAG.Player.Health;

namespace WAG.Player.Attacks
{
    public class AttackHitResponder : CompHitResponder
    {
        [Header("Values")] [SerializeField] private float attackMissCooldown = 2f;
        [SerializeField] private float attackMissSpeedModifier = -0.8f;
        [SerializeField] private float attackHitCooldown = 2.6f;
        [SerializeField] private float attackHitSpeedModifier = -0.9f;
        [SerializeField] private float chargeTime = 0.3f;
        [SerializeField] private float chargedAttackSpeedModifier = 0.5f;

        private bool canAttack = true;
        private PhotonView pv;

        private PlayerController pc;

        protected override void Awake()
        {
            base.Awake();
            if (!transform.parent.TryGetComponent<PlayerController>(out pc))
            {
                Debug.LogError("AttackHitResponder need to be chiled to PlayerController", this);
                Debug.Break();
            }

            InputManager.Instance.AddCallbackAction(
                ActionsControls.Catch,
                started: context =>
                {
                    /*Debug.Log("started: " + context);*/
                },
                performed: context =>
                {
                    if (!canAttack)
                        return;
                    canAttack = false;
                    if (hitBox.CheckHit(out HitData data))
                    {
                        Debug.Log("CheckHit = true");
                        if (data.HurtBox.Owner.parent.TryGetComponent<PlayerHealthController>(
                                out PlayerHealthController phc))
                        {
                            Debug.Log("PlayerHealthController = true");
                            phc.TakeDamage();
                            pc.SetTemporarySpeedForSeconds(
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

                    pc.SetTemporarySpeedForSeconds(
                        speedModifier: attackMissSpeedModifier,
                        duration: attackMissCooldown,
                        callBack: () =>
                        {
                            Debug.Log("attack miss callback = true");
                            canAttack = true;
                        });
                },
                canceled: context =>
                {
                    /* Debug.Log("canceled: "+context);*/
                });
        }
    }
}