using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    private Interactable interactableObject;
    // Start is called before the first frame update

    private GameObject player;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player") ;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Interact") && interactableObject != null)
        {
            interactableObject.onInteract?.Invoke(player);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
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
