using Photon.Pun;
using UnityEngine;
using WAG.Health;

namespace WAG.Player.Health
{
    public class PlayerHealthController : HealthStatusController
    {
        [SerializeField] private float healthySpeedModifier;
        [SerializeField] private float woundedSpeedModifier = -0.2f;
        [SerializeField] private float dyingSpeedModifier = -0.8f;

        public float HealthSpeedModifier { get; private set; } = 0;

        protected override void Awake()
        {
            base.Awake();
            pv.RPC(nameof(RPC_SyncHealth), RpcTarget.All, (int)startHeathStatus);
            OnHealthChanged += status =>  pv.RPC(nameof(RPC_SyncHealth), RpcTarget.All, (int)status);
        }
        [PunRPC]
        private void RPC_SyncHealth(int status)
        {
            SetHealth(status);
            if (!pv.IsMine)
                return;
            Invoke();
            
        }

        protected override void OnHealthy()
        {
            HealthSpeedModifier = healthySpeedModifier;
        }

        protected override void OnWounded()
        {
            HealthSpeedModifier = woundedSpeedModifier;
        }

        protected override void OnDying()
        {
            HealthSpeedModifier = dyingSpeedModifier;
        }

        protected override void OnDead()
        {
            HealthSpeedModifier = -1;
        }
    }
}