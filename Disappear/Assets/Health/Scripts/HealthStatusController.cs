using System;
using Photon.Pun;
using UnityEngine;

namespace WAG.Health
{
    public abstract class HealthStatusController : MonoBehaviour
    {
        public delegate void ChangeHealth(HeathStatus status);
        public event ChangeHealth OnHealthChanged;

        [SerializeField] protected HeathStatus startHeathStatus = HeathStatus.Healthy;

        [SerializeField] private bool recalculateHealth;
        
        private HeathStatus currentHeathStatus;
        public HeathStatus CurrentHeathStatus => currentHeathStatus;

        protected PhotonView pv;
        protected virtual void Awake()
        {
            if (!TryGetComponent<PhotonView>(out pv))
            {
                Debug.LogError("HealthStatusController need status", this);
                Debug.Break();
            }
         
            currentHeathStatus = startHeathStatus;
            
            OnHealthChanged += HealthChangeAction;
            Invoke();
        }

        protected virtual void OnDestroy()
        {
            OnHealthChanged -= HealthChangeAction;
        }

        private void Update()
        {
            if (recalculateHealth)
            {
                Invoke();
                recalculateHealth = false;
            }
        }

        protected void Invoke()
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
        public void SetHealth(HeathStatus newStatus)
        {
            if (newStatus == currentHeathStatus)
                return;

            currentHeathStatus = newStatus;
            
            Invoke();
        }

        /// <summary>
        /// Set health status 
        /// </summary>
        /// <param name="newStatus">Bounded</param>
        public void SetHealth(int newStatus)
        {
            SetHealth((HeathStatus) Math.Clamp(newStatus, 0, (int) HeathStatus.Healthy));
        }

        /// <summary>
        /// Set health status
        /// </summary>
        /// <param name="amount">Bounded</param>
        public void ChangeStatus(int amount)
        {
            SetHealth(amount + (int) currentHeathStatus);
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