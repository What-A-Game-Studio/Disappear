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
    [Header("Movement")] [Range(0.5f, 10)] [SerializeField]
    protected float moveSpeed = 5;

    protected float moveSpeedMultiplier = 2f;
    [SerializeField] protected float groundDrag = 6f;
    [SerializeField] protected LayerMask[] walkableMasks;
    [Header("Jumping")] [SerializeField] protected float jumpHeight = 1f;
    [SerializeField] protected float airDrag = 2f;
    [SerializeField] protected float airMultiplier = 0.4f;
    public bool IsGrounded { get; set; }

    protected float horizontalMovement;
    protected float verticalMovement;
    protected float jumpMovement;
    protected Vector3 moveDirection;
    protected Rigidbody rb;
    protected Transform groundChecker;
    protected MeshRenderer meshRenderer;

    private void Start()
    {
        if (!TryGetComponent<Rigidbody>(out rb))
        {
            Debug.LogError("PlayerMovement required a Rigidbody");
            Debug.DebugBreak();
        }

        meshRenderer = GetComponentInChildren<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("PlayerMovement required a Mesh in children");
            Debug.DebugBreak();
        }

        groundChecker = transform.Find("GroundChecker");
        if (groundChecker == null)
        {
            Debug.LogError("PlayerMovement required a GroundChecker in children");
            Debug.DebugBreak();
        }

        Vector3 positionGroundChecker = new Vector3(transform.position.x,
            transform.position.y - (meshRenderer.bounds.size.y / 2 - 0.15f),
            transform.position.z);
        Vector3 meshSize = meshRenderer.bounds.size * 0.9f;
        meshSize.y = 0.1f;
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = transform;
        cube.transform.position = positionGroundChecker;
        cube.transform.localScale = meshSize;

        rb.freezeRotation = true;
    }

    private void Update()
    {
        Inputs();
        ControlDrag();
    }

    /// <summary>
    /// Read Inputs & create move vector
    /// </summary>
    void Inputs()
    {
        verticalMovement = Input.GetAxisRaw("Vertical");
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        jumpMovement = Input.GetAxisRaw("Jump");
        //Create the direction
        moveDirection = transform.forward * verticalMovement
                        + transform.right * horizontalMovement
                        + (IsGrounded ? transform.up * jumpMovement : -(transform.up * jumpMovement)*airMultiplier) ;
    }

    private void FixedUpdate()
    {
        Ground();
        MovePlayer();
    }

    private void MovePlayer()
    {
        //Apply direction
        Vector3 velocity = moveDirection.normalized * moveSpeed * moveSpeedMultiplier;
        if (!IsGrounded)
        {
            velocity *= airMultiplier;
        }

        rb.velocity = velocity;
    }

    void Ground()
    {
        // Determine l'emplacement du ground checker en fonction du model 
        Vector3 positionGroundChecker = new Vector3(transform.position.x,
            transform.position.y - (meshRenderer.bounds.size.y / 2 - 0.15f),
            transform.position.z);
        Vector3 meshSize = meshRenderer.bounds.size * 0.9f;
        meshSize.y = 0.1f;
        // size.y = 0.1f;
        Collider[] colliders = Physics.OverlapBox(positionGroundChecker, meshSize, transform.rotation);
        IsGrounded = false;
        for (int i = 0; i < colliders.Length; i++)
        {
            for (int j = 0; j < walkableMasks.Length; j++)
            {
                if ((walkableMasks[j].value & (1 << colliders[i].gameObject.layer)) > 0)
                {
                    IsGrounded = true;
                    break;
                }
            }
        }
    }


    void ControlDrag()
    {
        Debug.Log(IsGrounded);
        if (IsGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = airDrag;
    }
}