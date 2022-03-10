using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
/// <summary>
/// Player movement physic based
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    [Range(0.5f,10)]
    [SerializeField] protected float moveSpeed = 5;

    [SerializeField] protected LayerMask walkableLayer;
    protected float moveSpeedMultiplier = 2f;
    private float rbDrag;

    protected float horizontalMovement;
    protected float verticalMovement;
    
    protected Vector3 moveDirection;
    protected Rigidbody rb;
    protected Transform groundChecker;
    private void Start()
    {
        if (!TryGetComponent<Rigidbody>(out rb))
        {
            Debug.LogError("PlayerMovement required a Rigidbody");
            Debug.DebugBreak();
        }

        groundChecker = transform.Find("GroundChecker");
        if (groundChecker == null)
        {
            Debug.LogError("PlayerMovement required a GroundChecker");
            Debug.DebugBreak();
        }
        rb.freezeRotation = true;
    }

    private void Update()
    {
        Inputs();
    }

    private void FixedUpdate()
    {
        Ground();
        //Apply direction
        rb.velocity = moveDirection.normalized * moveSpeed * moveSpeedMultiplier;
    }

    void Ground()
    {
        RaycastHit hit;
        Vector3 size = transform.localScale;
        size.y = 0.1f;
        Collider[] colliders = Physics.OverlapBox(groundChecker.position, size, quaternion.identity);

    }
    /// <summary>
    /// Read Inputs & create move vector
    /// </summary>
    void Inputs()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        //Create the direction
        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
    }

    void ControlDrag()
    {
        rb.drag = rbDrag;
    }
    
}
