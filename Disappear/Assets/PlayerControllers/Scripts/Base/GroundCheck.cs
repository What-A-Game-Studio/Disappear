using UnityEngine;

public class GroundCheck<T> : MonoBehaviour where T : MonoBehaviour, Groundable
{
    private T controller;
    
    protected virtual void Awake()
    {
        controller = GetComponentInParent<T>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = true;
    }

    protected virtual  void OnTriggerExit(Collider other)
    {
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = false;
    }

    protected virtual  void OnTriggerStay(Collider other)
    {
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = true;
    }

    protected virtual  void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = true;
    }

    protected virtual  void OnCollisionExit(Collision other)
    {        
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = false;
    }

    protected virtual  void OnCollisionStay(Collision other)
    {
        if (other.gameObject == controller.gameObject)
            return;
        controller.Grounded = true;
    }
}