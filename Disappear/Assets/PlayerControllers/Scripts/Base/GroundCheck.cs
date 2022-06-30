using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundCheck<T> : MonoBehaviour where T : MonoBehaviour, Groundable
{
    private T controller;
    private FootstepEvent stepEvent;
    
    void Awake()
    {
        controller = GetComponentInParent<T>();
        stepEvent = transform.parent.GetComponentInChildren<FootstepEvent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = true;
        if (other.TryGetComponent(out SurfaceController sc))
        {
            stepEvent.ChangeSurfaceType(sc.SurfaceType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = true;
    }

    private void OnCollisionExit(Collision other)
    {        
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = false;
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = true;
    }
}