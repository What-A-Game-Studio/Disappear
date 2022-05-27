using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour, Groundable
{
    [Header("Camera")] 
    [SerializeField] private GameObject cameraObject;
    
    [Header("Walking")] 
    [SerializeField] protected float speed;
    [SerializeField] protected float drag;
    private float horizontalInput, verticalInput;
    private Vector3 moveDirection;
    private const float SpeedModifier = 10f;
    private Transform orientation;
    
    [Header("Sprinting")] 
    [SerializeField] private float sprintSpeedMultiplier;
    
    [Header("Crouching")] 
    [SerializeField] private float crouchSpeedMultiplier;
    //Do the job for now 
    //Change when we have better model
    [SerializeField] private float crouchYScale;
    [SerializeField] private float crouchingDownForce;
    private float crouchOriginYScale;
    
    
    [Header("Jump")] 
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float jumpCooldown;
    [SerializeField]
    private float airMultiplier;
    private bool readyToJump = true;
    public bool Grounded { get; set; }
    
    [Header("Slope")] 
    [SerializeField]
    private float maxSlopeAngle = 45f;
    [SerializeField] 
    private float downForceSlope = 8f;
    [SerializeField]
    private LayerMask mask = 0;
    private RaycastHit slopeHit;
    private float angle;
    
    PhotonView pv;
    private Rigidbody rb;
    private CapsuleCollider collider;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (pv == null)
            throw new Exception("PlayerController required PhotonView !");
        
        if(!pv.IsMine) 
            return;
        
        if (cameraObject == null)
            throw new Exception("PlayerController required CameraHolderPrefab !");
        
        orientation = transform.Find("Orientation");
        if (orientation == null)
            throw new Exception("PlayerController required Orientation GameObject in theres children!");
        
        
        GameObject cameraHolder = Instantiate(cameraObject);
        CameraController cameraController = cameraHolder.GetComponent<CameraController>();
        cameraController.Orientation = orientation;
        
        
        collider = GetComponent<CapsuleCollider>(); 
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Start is called before the first frame update
    private void Start()
    {
        crouchOriginYScale = transform.localScale.y;
    }

    // Update is called once per frame
    private void Update()
    {
        if(!pv.IsMine)
            return;
        
        InputsControls();
        SpeedControl();
        if (Grounded)
            rb.drag = drag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        if(!pv.IsMine)
            return;
        Move();
    }

    
    /// <summary>
    /// Handle user input
    /// </summary>
    private void InputsControls()
    {     
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && readyToJump && Grounded)
        {
            readyToJump = false;
            Jump();
            StartCoroutine(ResetJump());
        }

        Vector3 localScale = transform.localScale;
        if (Input.GetButtonDown("Crouch"))
        {
            localScale = new Vector3(localScale.x, crouchYScale, localScale.z);
            transform.localScale = localScale;
            rb.AddForce(Vector3.down * crouchingDownForce, ForceMode.Impulse);
        }

        if (Input.GetButtonUp("Crouch"))
        {
            transform.localScale = new Vector3(localScale.x, crouchOriginYScale, localScale.z);
        }
    }

    /// <summary>
    /// Apply physics based of InputsControls
    /// </summary>
    protected virtual void Move()
    {
        //calculate dir 
        moveDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput);
        Vector3 movDir;
        
        //on slope we turn off gravity
        //for avoid slip down
        bool onSlope = OnSlope();
        rb.useGravity = !onSlope;
        if (onSlope)
        {
            if (angle < maxSlopeAngle)
            {
                movDir = GetSlopeMoveDirection();
            }
            else
            {
                movDir = Vector3.zero;
            }

            if(rb.velocity.y > 0f)
                rb.AddForce(Vector3.down * downForceSlope, ForceMode.Force);
        }
        else
        {
            movDir = moveDirection.normalized;
        }
        rb.AddForce(movDir * (GetSpeed() * SpeedModifier), ForceMode.Force);
    }
    
    /// <summary>
    /// "clamp" speed at max speed 
    /// </summary>
    private void SpeedControl()
    {
        float currentSpeed = GetSpeed();
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > currentSpeed)
            {
                flatVel = flatVel.normalized * currentSpeed;
                rb.velocity = new Vector3(flatVel.x, rb.velocity.y, flatVel.z);
            }
    }

    /// <summary>
    /// Calculate player speed based on Inputs & position
    /// </summary>
    /// <returns>Speed value</returns>
    private float GetSpeed()
    {
        float resultSpeed = speed;
        if (Grounded && Input.GetButton("Sprint"))
        {
            resultSpeed *= sprintSpeedMultiplier;
        }
        if (!Grounded)
        {
            resultSpeed *= airMultiplier;
        }

        if (Grounded && Input.GetButton("Crouch"))
        {
            resultSpeed *= crouchSpeedMultiplier;
        }

        if (angle > maxSlopeAngle)
        {
            resultSpeed = 0f;
        }
        return resultSpeed;
    }
    
    /// <summary>
    /// Apply Jump physics
    /// </summary>
    private void Jump()
    {
        //rest y velocity
        Vector3 velocity = rb.velocity;
        velocity = new Vector3(velocity.x, 0f, velocity.z);
        rb.velocity = velocity;
        rb.AddForce(transform.up* jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    ///  After seconds reactive jump & store slope angle in "angle"
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(jumpCooldown);
        readyToJump = true;
    }
    
    /// <summary>
    /// Check if player is on slope
    /// </summary>
    /// <returns></returns>
    private bool OnSlope()
    {
        angle = 0;
        Debug.DrawRay(transform.position,Vector3.down*(collider.height/2f+0.3f), Color.red);

        if (Physics.Raycast(transform.position,Vector3.down, out slopeHit, collider.height/2f+0.3f,mask))
        {
            angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    /// <summary>
    /// Calculate direction based on slope 
    /// </summary>
    /// <returns></returns>
    private Vector3 GetSlopeMoveDirection()
    {
        
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}