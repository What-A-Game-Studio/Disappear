using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    #region Class Members

    [Header("Interaction Parameters")] 

    [Tooltip("Layer of objects that can be interacted with")] [SerializeField]
    private LayerMask layer;

    [Tooltip("Maximum distance to detect interactable objects")] [SerializeField]
    private int interactMaxDistance;

    public Interactable interactableObject;
    private Transform cam;
    private RaycastHit hit;
    private Ray camRay;
    private GameObject player;

    // Start is called before the first frame update

    #endregion

    #region Unity Functions

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cam = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact") && interactableObject != null)
        {
            interactableObject.onInteract?.Invoke(player);
        }
    }

    void FixedUpdate()
    {
        camRay = new Ray(cam.position, cam.forward);

        if (Physics.Raycast(camRay, out hit, interactMaxDistance, layer))
        {
            Debug.Log("Found");
            if (!hit.collider.TryGetComponent(out interactableObject))
            {
                interactableObject = null;
            }
        }
        else
        {
            interactableObject = null;
        }
    }

    #endregion
}