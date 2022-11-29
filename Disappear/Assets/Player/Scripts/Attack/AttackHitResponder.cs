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

        private float timeSinceStartedAttack = 0f;
        private bool attackStarted;

        private PhotonView pv;

        private PlayerController pc;

        protected override void Awake()
        {
            base.Awake();
            InputManager.Instance.AddCallbackAction(
                ActionsControls.Catch,
                started: context =>
                {
                    /*Debug.Log("started: " + context);*/
                },
                performed: context =>
                {
                    if (hitBox.CheckHit(out HitData data))
                    {
                        if (data.HurtBox.Owner.parent.TryGetComponent<PlayerHealthController>(
                                out PlayerHealthController phc))
                        {
                            phc.TakeDamage();
                        }
                    }
                },
                canceled: context =>
                {
                    /* Debug.Log("canceled: "+context);*/
                });
        }
    }
}