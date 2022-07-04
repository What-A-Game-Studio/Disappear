using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(
    typeof(PhotonView)
)]
public class PlayerController : MonoBehaviour, Groundable
{
    public static PlayerController MainPlayer { get; protected set; }

    [Header("Camera")] [SerializeField] private float cameraSpeed = 300f;

    [Header("Walking")] [SerializeField] protected float speed;
    protected float teamSpeedModifier;
    [SerializeField] protected float drag;
    private float horizontalInput, verticalInput;
    private Vector3 moveDirection;
    private const float SpeedModifier = 10f;
    public Transform OrientationTransform { get; protected set; }

    [Header("Sprinting")] [SerializeField] private float sprintSpeedMultiplier;

    [Header("Crouching")] [SerializeField] private float crouchSpeedMultiplier;

    //Do the job for now 
    //Change when we have better model
    [SerializeField] private float crouchYScale;
    [SerializeField] private float crouchingDownForce;
    private float crouchOriginYScale;


    [Header("Jump")] [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump = true;
    public bool Grounded { get; set; }

    [Header("Slope")] [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private float downForceSlope = 8f;
    [SerializeField] private LayerMask mask = 0;
    private RaycastHit slopeHit;
    private float angle;

    [Header("Inventory")] [SerializeField] private GameObject gameUI;

    public PhotonView Pv { get; private set; }
    private Rigidbody rb;

    public Vector3 PlayerVelocity
    {
        get { return Pv.IsMine ? rb.velocity : distVelocity; }
    }

    private Vector3 distVelocity;
    private CapsuleCollider collider;
    public PlayerInventory PlayerInventory { get; protected set; }
    public CameraController CameraController { get; protected set; }
    [Header("DEBUG")] public bool isSeeker = true;

    private void Awake()
    {
        Pv = GetComponent<PhotonView>();
        if (Pv == null)
            throw new Exception("PlayerController required PhotonView !");

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        InitModel();

        if (!Pv.IsMine)
            return;
        Init();
    }

    private void InitModel()
    {
        PlayerAnimationController pac = GetComponent<PlayerAnimationController>();
        name = PhotonNetwork.LocalPlayer.NickName;
        TeamController tc = GetComponent<TeamController>();
        tc.SetTeamData(Equals(PhotonNetwork.MasterClient, pv.Owner), pac, pv);
    }

    private void Init()
    {
        MainPlayer = this;

        OrientationTransform = transform.Find("CameraHolder");
        if (OrientationTransform == null)
            throw new Exception("PlayerController required CameraHolder GameObject in theres children!");

        PlayerInventory = gameObject.AddComponent<PlayerInventory>();
        PlayerInventory.Init(gameUI);


        CameraController = Camera.main.transform.parent.GetComponent<CameraController>();
        CameraController.SetOrientation(OrientationTransform);
        CameraController.Speed = cameraSpeed;
        Camera.main.GetComponentInChildren<PlayerInteraction>()?.Init(gameObject, isSeeker);


        collider = GetComponent<CapsuleCollider>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        crouchOriginYScale = transform.localScale.y;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!Pv.IsMine)
            return;

        InputsControls();
        SpeedControl();
        if (Grounded)
            rb.drag = drag;
        else
            rb.drag = 0;
        transform.rotation = CameraController.GetOrientationRotation();
    }

    private void FixedUpdate()
    {
        if (!Pv.IsMine)
            return;
        Move();
        Pv.RPC(nameof(RPC_Velocity), RpcTarget.All, rb.velocity);
    }

    [PunRPC]
    private void RPC_Velocity(Vector3 vel)
    {
        distVelocity = vel;
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
        moveDirection = (OrientationTransform.forward * verticalInput + OrientationTransform.right * horizontalInput);
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

            if (rb.velocity.y > 0f)
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

        return resultSpeed * teamSpeedModifier;
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
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
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
        Debug.DrawRay(transform.position, Vector3.down * (collider.height / 2f + 0.3f), Color.red);

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, collider.height / 2f + 0.3f, mask))
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

    public void SetTeamSpeedModifier(float teamDataSpeedModifier)
    {
        teamSpeedModifier = teamDataSpeedModifier;
    }

    public void Teleport()
    {
        Pv.RPC(nameof(RPC_Teleport), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Teleport()
    {
        if (!Pv.IsMine)
            return;
        transform.position = PlayerSpawnerManager.Instance.ChooseRandomSpawnPosition();
    }

    public void Defeat()
    {
        Pv.RPC(nameof(RPC_Defeat), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Defeat()
    {
        if (!Pv.IsMine)
            return;
        MenuManager.Instance.OpenMenu(MenuType.Pause);
    }

    public bool IsMine()
    {
        return Pv.IsMine;
    }
}