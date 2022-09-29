using UnityEngine;
using Photon.Pun;
using System;

[RequireComponent(
    typeof(Rigidbody),
    typeof(CapsuleCollider)
)]
public class PlayerController : MonoBehaviour
{
    public static PlayerController MainPlayer { get; private set; }

    private Rigidbody rb;

    private Animator animator;
    private Vector3 currentVelocity;
    private float xRotation;
    private float teamSpeedModifier = 0;
    private PlayerAnimationController pac;
    public PlayerInventory PlayerInventory { get; private set; }
    public PhotonView Pv { get; private set; }

    [Header("Camera")] [SerializeField] private Transform cameraRig;
    [SerializeField] private Transform cam;
    [SerializeField] private float mouseSensitivity = 22f;
    [SerializeField] private float upperLimit = -40f;
    [SerializeField] private float bottomLimit = 70f;

    [Header("Walk")] [SerializeField] private float walkSpeed = 2f;
    [Header("Run")] [SerializeField] private float runSpeedFactor = 0.5f;
    [Header("Crouch")] [SerializeField] private float crouchSpeedFactor = -0.5f;
    private bool rpcCrouch;
    public bool Crouched => Pv.IsMine ? InputManager.Instance.Crouch : rpcCrouch;

    private bool grounded;
    private bool rpcGrounded;
    public bool Grounded => Pv.IsMine ? grounded : rpcGrounded;

    [Header("Jump")] [SerializeField] [Range(100, 1000)]
    private float jumpFactor = 260f;

    [SerializeField] private float airResistance = 0.8f;
    [SerializeField] private LayerMask groundCheck;

    [Header("Others")] [SerializeField] private float animBlendSpeed = 8.9f;
    [SerializeField] private float dis2Ground = 0.8f;

    [Header("Inventory")] [SerializeField] private GameObject gameUI;
    private bool inventoryStatus;
    private bool rpcInventoryStatus;
    public bool InventoryStatus => Pv.IsMine ? inventoryStatus : rpcInventoryStatus;

    private Vector3 rpcVelocity;
    public Vector3 PlayerVelocity => Pv.IsMine ? currentVelocity : rpcVelocity;


    public bool CanMoveOrRotate { get; set; } = true;

    #region Unity Events

    void Awake()
    {
        HideCursor();
        if (!TryGetComponent<Animator>(out animator))
        {
            Debug.LogError("Need animator", this);
            Debug.Break();
        }

        rb = GetComponent<Rigidbody>();
        Pv = GetComponent<PhotonView>();
        if (Pv == null)
            throw new Exception("PlayerController required PhotonView !");

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        InitModel();
        pac = gameObject.AddComponent<PlayerAnimationController>();
        pac.PC = this;
        if (!Pv.IsMine)
            return;

        Init();
        // cam.position = cameraRig.position;
        // cam.rotation = cameraRig.rotation;
    }

    private void FixedUpdate()
    {
        SampleGround();
        if (!Pv.IsMine)
            return;

        if (CanMoveOrRotate)
        {
            Move();
            HandleJump();
            HandleInventory();
            HandleInteract();
        }

        Pv.RPC(nameof(RPC_Velocity), RpcTarget.All, currentVelocity);
        Pv.RPC(nameof(RPC_Crouch), RpcTarget.All, Crouched);
        Pv.RPC(nameof(RPC_Ground), RpcTarget.All, Grounded);
    }

    //After FixedUpdate
    private void LateUpdate()
    {
        if (!Pv.IsMine)
            return;

        if (CanMoveOrRotate)
            CameraMovement();
    }

    #endregion Unity Events

    #region Privates

    private void InitModel()
    {
        // PlayerAnimationController pac = GetComponent<PlayerAnimationController>();
        name = PhotonNetwork.LocalPlayer.NickName;
        TeamController tc = GetComponent<TeamController>();
        tc.SetTeamData(Equals(PhotonNetwork.MasterClient, Pv.Owner), Pv, ref cameraRig);
    }

    private void Init()
    {
        MainPlayer = this;
        cam = Camera.main.transform;
        cam.parent = transform;
        PlayerInventory = gameObject.AddComponent<PlayerInventory>();
        PlayerInventory.Init(gameUI, gameObject);

        cam.GetComponentInChildren<PlayerInteraction>()?.Init(gameObject, true);
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void HandleInventory()
    {
        if (!InputManager.Instance.Inventory)
            return;
        inventoryStatus = !inventoryStatus;
        Pv.RPC(nameof(RPC_InventoryStatus), RpcTarget.All, Grounded);
    }

    private void HandleInteract()
    {
        Debug.Log("HandleInteract");
        if (!InputManager.Instance.Interact)
            return;

        Pv.RPC(nameof(RPC_Interact), RpcTarget.All);
    }

    private void Move()
    {
        float targetSpeed = walkSpeed;

        if (InputManager.Instance.Move == Vector2.zero)
            targetSpeed = 0f;

        if (InputManager.Instance.Run)
            targetSpeed += targetSpeed * runSpeedFactor;
        if (Crouched)
            targetSpeed += targetSpeed * crouchSpeedFactor;

        if (grounded)
        {
            currentVelocity.x = Mathf.Lerp(currentVelocity.x,
                targetSpeed * InputManager.Instance.Move.x,
                animBlendSpeed * Time.fixedDeltaTime);
            currentVelocity.z = Mathf.Lerp(currentVelocity.z,
                targetSpeed * InputManager.Instance.Move.y,
                animBlendSpeed * Time.fixedDeltaTime);

            float xVelDiff = currentVelocity.x - rb.velocity.x;
            float zVelDiff = currentVelocity.z - rb.velocity.z;

            rb.AddForce(transform.TransformVector(new Vector3(xVelDiff, 0, zVelDiff)),
                ForceMode.VelocityChange);
        }
        else
        {
            rb.AddForce(
                transform.TransformVector(currentVelocity.x * airResistance, 0, currentVelocity.z * airResistance),
                ForceMode.VelocityChange);
        }
    }

    private void CameraMovement()
    {
        if (!cam)
            return;

        float mouseX = InputManager.Instance.Look.x;
        float mouseY = InputManager.Instance.Look.y;


        cam.position = cameraRig.position;
        xRotation -= mouseY * mouseSensitivity * Time.smoothDeltaTime;
        xRotation = Mathf.Clamp(xRotation, upperLimit, bottomLimit);
        //Up & down vision 
        cam.localRotation = Quaternion.Euler(xRotation, 0, 0);


        rb.MoveRotation(rb.rotation * (Quaternion.Euler(0, mouseX * mouseSensitivity * Time.fixedDeltaTime, 0)));
    }

    private void HandleJump()
    {
        if (!InputManager.Instance.Jump)
            return;
        if (!grounded)
            return;
        animator.SetTrigger(PlayerAnimationController.JumpHash);
    }


    /// <summary>
    /// Add force for jump, Used in Animation Event
    /// </summary>
    public void JumpAddForce()
    {
        rb.AddForce(-rb.velocity.y * Vector3.up, ForceMode.VelocityChange);
        rb.AddForce(Vector3.up * jumpFactor, ForceMode.Impulse);
        animator.ResetTrigger(PlayerAnimationController.JumpHash);
    }


    private void SampleGround()
    {
        grounded = Physics.Raycast(
            rb.worldCenterOfMass,
            Vector3.down,
            out RaycastHit hitInfos,
            dis2Ground + 0.2f,
            groundCheck);
        if (!grounded)
        {   
            currentVelocity.y = rb.velocity.y;
        }
        
    }

    #endregion Private

    #region Public

    public void SetCameraRig(Transform cr)
    {
        cameraRig = cr;
    }

    public void SetTeamSpeedModifier(float teamDataSpeedModifier)
    {
        teamSpeedModifier = teamDataSpeedModifier;
    }

    public bool IsMine()
    {
        return Pv.IsMine;
    }

    public void Defeat()
    {
        Pv.RPC(nameof(RPC_Defeat), RpcTarget.All);
    }

    #endregion Public

    #region RPC

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


    [PunRPC]
    private void RPC_Defeat()
    {
        if (!Pv.IsMine)
            return;
        MenuManager.Instance.OpenMenu(MenuType.Pause);
    }

    [PunRPC]
    private void RPC_Velocity(Vector3 vel)
    {
        rpcVelocity = vel;
    }

    [PunRPC]
    private void RPC_Crouch(bool crouch)
    {
        rpcCrouch = crouch;
    }

    [PunRPC]
    private void RPC_Ground(bool ground)
    {
        rpcGrounded = ground;
    }

    [PunRPC]
    private void RPC_InventoryStatus(bool inventoryStatus)
    {
        rpcInventoryStatus = inventoryStatus;
    }

    [PunRPC]
    private void RPC_Interact()
    {
        Debug.Log("Interact");
        pac.InteractTrigger();
    }

    #endregion RPC
}