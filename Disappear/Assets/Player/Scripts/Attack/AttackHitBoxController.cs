using UnityEngine;
using WAG.Health;

namespace WAG.Player.Attacks
{
    public class AttackHitBoxController : MonoBehaviour
    {
        [SerializeField] private HealthStatusController damageableObjectInRange = null;

        public HealthStatusController DamageableObjectInRange => damageableObjectInRange;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<HealthStatusController>(out HealthStatusController tmp))
            {
                // Ray r = new Ray(transform.position, (tmp.transform.position - transform.position).normalized);
                // if (Physics.Raycast(r, out RaycastHit hitInfo))
                // {
                //     if (tmp.gameObject.GetInstanceID() == hitInfo.collider.gameObject.GetInstanceID())
                //     {
                if (!damageableObjectInRange ||
                    tmp.gameObject.GetInstanceID() != damageableObjectInRange.gameObject.GetInstanceID())
                {
                    damageableObjectInRange = tmp;
                    Debug.Log(damageableObjectInRange.gameObject.name);
                }
                //     }
                // }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (damageableObjectInRange != null &&
                other.gameObject.GetInstanceID() == damageableObjectInRange.gameObject.GetInstanceID())
            {
                damageableObjectInRange = null;
            }
        }
    }
}