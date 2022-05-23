using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerControllerBis : MonoBehaviour, Groundable
{
    [Header("Camera")] 
    [SerializeField] private GameObject cameraObject;
    [Header("Walking")] 
    [SerializeField] protected float speed;
    [SerializeField] protected float drag;
    [Header("Sprinting")] 
    [SerializeField] private float sprintSpeedMultiplier;
    [Header("Crouching")] 
    [SerializeField] private float crouchSpeedMultiplier;
    //Do the job for now 
    //Change when we have better model
    [SerializeField] private float crouchYScale;
    [SerializeField] private float crouchingDownForce;
    private float crouchOriginYScale;
    
    private const float SpeedModifier = 10f;
    private Transform orientation;
    
    [Header("Jump")] 
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float jumpCooldown;
    [SerializeField]
    private float airMultiplier;
    private bool readyToJump = true;
    
    [Header("DEBUG")]
    [SerializeField]
    private float currentSpeed = 0;
    [field:SerializeField]
    public bool Grounded { get; set; }
    
    
    PhotonView pv;
    private Rigidbody rb;
    private CapsuleCollider collider;
    private float horizontalInput, verticalInput;
    private Vector3 moveDirection;

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
        
        currentSpeed = rb.velocity.magnitude;
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

    
    protected virtual void InputsControls()
    {     
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && readyToJump && Grounded)
        {
            readyToJump = false;
            
            Jump();

            StartCoroutine(ResetJump());
        }

        if (Input.GetButtonDown("Crouch"))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * crouchingDownForce, ForceMode.Impulse);
            
        }

        if (Input.GetButtonUp("Crouch"))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchOriginYScale, transform.localScale.z);
        }
    }

    protected virtual void Move()
    {
        //calculate dir 
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;        
        rb.AddForce(moveDirection.normalized * (GetSpeed() * SpeedModifier), ForceMode.Force);
    }
    /// <summary>
    /// "clamp" speed at max speed 
    /// </summary>
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float currentSpeed = GetSpeed();
        if (flatVel.magnitude > currentSpeed)
        {
            flatVel = flatVel.normalized * currentSpeed;
            rb.velocity = new Vector3(flatVel.x, rb.velocity.y, flatVel.z);
        }
    }


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
        return resultSpeed;
    }
    
    private void Jump()
    {
        //rest y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up* jumpForce, ForceMode.Impulse);
    }

    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(jumpCooldown);
        readyToJump = true;
    }
}