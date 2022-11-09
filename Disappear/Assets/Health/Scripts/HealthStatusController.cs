using UnityEngine;

namespace WAG.Health
{
    public class HealthStatusController : MonoBehaviour
    {
        public delegate void ChangeHealth(HeathStatus status);

        public event ChangeHealth OnHealthChanged;

        [SerializeField] private HeathStatus startHeathStatus;

        private HeathStatus currentHeathStatus;
        public HeathStatus CurrentHeathStatus => currentHeathStatus;

        private void Awake()
        {
            currentHeathStatus = startHeathStatus;
        }

        public void HealUp()
        {
            if (currentHeathStatus < HeathStatus.Healthy)
                SetHealth(currentHeathStatus + 1);
        }

        public void SetHealth(HeathStatus newStatus)
        {
            currentHeathStatus = newStatus;
            if (OnHealthChanged != null)
                OnHealthChanged(currentHeathStatus);
        }

        public void TakeDamage()
        {
            if (currentHeathStatus > HeathStatus.Dead)
                SetHealth(currentHeathStatus - 1);
        }
    }
}