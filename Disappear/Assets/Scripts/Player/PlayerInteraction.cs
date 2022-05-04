using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    private Interactable interactableObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && interactableObject != null)
        {
            Debug.Log("Input");
            interactableObject.onInteract?.Invoke(this.transform.parent.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detect trigger");
        if (other.CompareTag("Interactable"))
        {
            Debug.Log("Detect Interactable");
            if (!other.TryGetComponent(out interactableObject))
            {
                interactableObject = null;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            interactableObject = null;
        }
    }
}
