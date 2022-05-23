using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour, Groundable
{
    [Header("Camera")] 
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private float mouseSpeed=1.0f;
    
    [Header("Move")]
    [SerializeField] private float speedBase;
    [SerializeField] private float airSpeedMultiplier;

    
    [Header("Sprint")]
    [SerializeField] private float sprintSpeedMultiplier;
    
    [SerializeField] private float sprintRecoverTime;
    [SerializeField] private float staminaMax;
    [SerializeField]  private float currentStamina;
    [SerializeField] private float staminaSpentPerSeconds;
    [SerializeField] private float staminaRecoverPerSeconds;
    
    [Header("Crouch")]
    [SerializeField] private float crouchSpeedMultiplier;
    
    [Header("Lie")]
    [SerializeField] private float lieSpeedMultiplier;
    [SerializeField] private float smoothTime;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce;

    [Header("Slop")] 
    [SerializeField]
    private float maxSlopeAngle = 45f;
    [SerializeField] 
    private float downForceSlope = 80f;
    [SerializeField]
    private LayerMask mask = 0;
    private RaycastHit slopeHit;
    
    private float verticalLookRotation;
    public bool Grounded { get; set; }

    private bool jump;
    
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;

    private Rigidbody rb;
    private CapsuleCollider collider;
    PhotonView pv;

    void Awake()
    {
        collider = GetComponent<CapsuleCollider>(); 
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //Is mine mean is local player
        if (!pv.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            return;
        }

        currentStamina = staminaMax;

    }

    void Update()
    {
        if(!pv.IsMine)
            return;
        Look();
        MoveControls();
        JumpControls();
        //Debug.DrawRay(transform.position,Vector3.down * (collider.height/2f+0.2f), Color.red);
    }


    private void FixedUpdate()
    {
        if(!pv.IsMine)
            return;
        Move();
        Jump();

    }
    
    private void JumpControls()
    {
        jump = Input.GetButton("Jump") && Grounded;
    }

    private void Jump()
    {
        if (jump)
        {
            //Reset the Y velocity to make sure the jump height is the same every time
            Vector3 v = rb.velocity;
            v = new Vector3(v.x, 0f, v.z);
            rb.velocity = v;
            
            rb.AddForce(transform.up * jumpForce);
            Grounded = false;
            jump = true;
        }
    }
    private void MoveControls()
    {
        if (!OnSlope(true))
        {
            moveAmount = Vector3.zero;
            return;
        }
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        
            moveAmount = Vector3.SmoothDamp(moveAmount,
                moveDir * GetSpeed(),
                ref smoothMoveVelocity,
                smoothTime);
    }

    private float GetSpeed()
    {
        float calculateSpeed = speedBase;
        if (!Grounded)
            return calculateSpeed * airSpeedMultiplier;
        if (Input.GetButton("Lie"))
        {
            calculateSpeed *= lieSpeedMultiplier;
            return calculateSpeed;
        }

        if (Input.GetButton("Sprint"))
        {
            calculateSpeed *= sprintSpeedMultiplier;
            currentStamina -= staminaSpentPerSeconds * Time.deltaTime;
        }
        else if(currentStamina <= staminaMax)
        {
            currentStamina += staminaRecoverPerSeconds * Time.deltaTime;
        }

        if (Input.GetButton("Crouch"))
        {
            calculateSpeed *= crouchSpeedMultiplier;
        }

        return calculateSpeed;
    }
    

    private void Move()
    {
        bool onSlope = OnSlope();
        rb.useGravity = !onSlope;
        if (onSlope && rb.velocity.y > 0)
        {
            rb.AddForce(Vector3.down * downForceSlope, ForceMode.Force);
        }
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        
    }
    void Look()
    {
        //Rotate Character
        transform.Rotate(Vector3.up * (Input.GetAxisRaw("Mouse X") * mouseSpeed));
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSpeed;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }
    
    private bool OnSlope(bool checkCanWalk = false)
    {
        
        if (Physics.Raycast(transform.position,Vector3.down, out slopeHit, collider.height/2f+0.3f,mask))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            if(!checkCanWalk)
                return angle != 0;
            else
                return angle < maxSlopeAngle;
        }
        return false;
    }


}