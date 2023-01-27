using System;
using UnityEngine;
using UnityEngine.UI;
using WAG.Core.Controls;
using WAG.Interactions;

namespace WAG.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        // [SerializeField] private BoxCollider box;
        [SerializeField] protected LayerMask hitLayer;

        private Interactable interactableObject;

        private void Start()
        {
            InputManager.Instance.AddCallbackAction(
                ActionsControls.Interact,
                (context) =>
                {
                    if (interactableObject != null)
                    {
                        interactableObject.onInteract?.Invoke(NGOPlayerController.MainPlayer.gameObject);
                    }
                });
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((hitLayer.value & (1 << other.gameObject.layer)) > 0)
                interactableObject = other.GetComponent<Interactable>();
        }



        private void OnTriggerExit(Collider other)
        {
            //Compare sur les bit et non sur le texte plus rapide
            if (interactableObject && (hitLayer.value & (1 << other.gameObject.layer)) > 0)
                if (interactableObject.gameObject.GetInstanceID() == other.gameObject.GetInstanceID())
                {
                    interactableObject = null;
                }
        }
    }
}