using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class ExitController : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out HiderController hc))
        {
            ExitHider(hc);
        }
    }

    protected void ExitHider(HiderController hc)
    {
        GameManager.Instance.HiderExit(hc);
    }
}
