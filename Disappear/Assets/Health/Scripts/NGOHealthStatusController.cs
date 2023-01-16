using System;
using Unity.Netcode;
using UnityEngine;

namespace WAG.Health
{
    public abstract class NGOHealthStatusController : NetworkBehaviour
    {
        public delegate void ChangeHealth(HeathStatus status);

        public event ChangeHealth OnHealthChanged;

        [SerializeField] protected HeathStatus startHeathStatus = HeathStatus.Healthy;

        [SerializeField] private bool recalculateHealth;

        private HeathStatus currentHeathStatus;
        public HeathStatus CurrentHeathStatus => currentHeathStatus;


        protected virtual void Awake()
        {
            currentHeathStatus = startHeathStatus;

            OnHealthChanged += HealthChangeAction;
            Invoke();
        }

        public override void OnDestroy()
        {
            OnHealthChanged -= HealthChangeAction;
        }

        public void Invoke()
        {
            OnHealthChanged?.Invoke(currentHeathStatus);
        }


        private void HealthChangeAction(HeathStatus status)
        {
            currentHeathStatus = status;
            switch (status)
            {
                case HeathStatus.Healthy:
                    OnHealthy();
                    break;
                case HeathStatus.Wounded:
                    OnWounded();
                    break;
                case HeathStatus.Dying:
                    OnDying();
                    break;
                case HeathStatus.Dead:
                default:
                    OnDead();
                    break;
            }
        }

        /// <summary>
        /// Call once when OnHealthChanged & currentHeathStatus == Healthy
        /// </summary>
        protected abstract void OnHealthy();

        /// <summary>
        /// Call once when OnHealthChanged & currentHeathStatus == Wounded
        /// </summary>
        protected abstract void OnWounded();

        /// <summary>
        /// Call once when OnHealthChanged & currentHeathStatus == Dying
        /// </summary>
        protected abstract void OnDying();

        /// <summary>
        /// Call once when OnHealthChanged & currentHeathStatus == Dead
        /// Default value if status has wrong value
        /// </summary>
        protected abstract void OnDead();

        /// <summary>
        /// Increase health status (bounded)
        /// </summary>
        public void HealUp()
        {
            if (currentHeathStatus < HeathStatus.Healthy)
                SetHealth(currentHeathStatus + 1);
        }

        /// <summary>
        /// Set health status
        /// </summary>
        /// <param name="newStatus"></param>
        /// <param name="invoke">Fire callback</param>
        public void SetHealth(HeathStatus newStatus, bool invoke = true)
        {
            if (newStatus == currentHeathStatus)
                return;

            currentHeathStatus = newStatus;
            if (invoke)
                Invoke();
        }

        /// <summary>
        /// Set health status 
        /// </summary>
        /// <param name="newStatus">Bounded</param>
        /// <param name="invoke">Fire callback</param>
        public void SetHealth(int newStatus, bool invoke = true)
        {
            SetHealth((HeathStatus)Math.Clamp(newStatus, 0, (int)HeathStatus.Healthy), invoke);
        }

        /// <summary>
        /// Set health status
        /// </summary>
        /// <param name="amount">Bounded</param>
        public void ChangeStatus(int amount)
        {
            SetHealth(amount + (int)currentHeathStatus);
        }

        /// <summary>
        /// decrease health status (bounded)
        /// </summary>
        public void TakeDamage()
        {
            if (currentHeathStatus > HeathStatus.Dead)
                SetHealth(currentHeathStatus - 1);
        }
    }
}