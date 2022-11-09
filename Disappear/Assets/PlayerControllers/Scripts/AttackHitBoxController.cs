using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using WAG.Health;

public class AttackHitBoxController : MonoBehaviour
{
    [SerializeField] private HealthStatusController damageableObjectInRange;

    private HealthStatusController DamageableObjectInRange => damageableObjectInRange;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<HealthStatusController>(out HealthStatusController tmp))
        {
            Ray r = new Ray(transform.position, (tmp.transform.position - transform.position).normalized);
            if (Physics.Raycast(r, out RaycastHit hitInfo))
            {
                if (tmp.gameObject.GetInstanceID() == hitInfo.collider.gameObject.GetInstanceID())
                {
                    if (!damageableObjectInRange || tmp.gameObject.GetInstanceID() != damageableObjectInRange.gameObject.GetInstanceID())
                    {
                        damageableObjectInRange = tmp;  
                        Debug.Log(damageableObjectInRange.gameObject.name);
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        damageableObjectInRange = null;
    }
}