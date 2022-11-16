using Photon.Pun;
using UnityEngine;
using WAG.Core.Controls;

namespace WAG.Player.Attacks
{
    public class AttackController : MonoBehaviour
    {
        [SerializeField] private AttackHitBoxController loadedAttack;
        [SerializeField] private AttackHitBoxController normalAttack;
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

        private void Awake()
        {
            ///TODO: Pas hyper clean je pense
            if (!transform.parent.parent.TryGetComponent<PhotonView>(out pv))
            {
                Debug.LogError("Need PhotonView", this);
                Debug.Break();
            }

            if (!transform.parent.parent.TryGetComponent<PlayerController>(out pc))
            {
                Debug.LogError("Need PlayerController", this);
                Debug.Break();
            }

            InputManager.Instance.AddCallbackAction(
                ActionsControls.Catch,
                started: context => { },
                performed: context => registerAttack(),
                canceled: context => { });
        }


        private void registerAttack()
        {
            if (loadedAttack.DamageableObjectInRange == null)
                return;

            Vector3 attackOrigin = loadedAttack.transform.position;
            Ray r = new Ray(attackOrigin, loadedAttack.DamageableObjectInRange.transform.position - attackOrigin);
            if (Physics.Raycast(r, out RaycastHit hitInfo) &&
                loadedAttack.DamageableObjectInRange.gameObject.GetInstanceID() ==
                hitInfo.collider.gameObject.GetInstanceID())
            {
                loadedAttack.DamageableObjectInRange.TakeDamage();
            }
        }
    }
}