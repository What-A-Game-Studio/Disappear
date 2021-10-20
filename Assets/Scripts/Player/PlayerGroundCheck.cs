using System;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    //[SerializeField] private LayerMask groundLayer;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerNotGO(other.gameObject))
            return;
        playerController.SetGroundedState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayerNotGO(other.gameObject))
            return;
        playerController.SetGroundedState(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsPlayerNotGO(other.gameObject))
            return;
        playerController.SetGroundedState(true);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (IsPlayerNotGO(other.gameObject))
            return;
        playerController.SetGroundedState(true);
    }

    private void OnCollisionExit(Collision other)
    {
        if (IsPlayerNotGO(other.gameObject))
            return;
        playerController.SetGroundedState(false);
    }

    private void OnCollisionStay(Collision other)
    {
        if (IsPlayerNotGO(other.gameObject))
            return;
        playerController.SetGroundedState(true);
    }

    private bool IsPlayerNotGO(GameObject go)
    {
        return playerController.gameObject != go;
    }
}