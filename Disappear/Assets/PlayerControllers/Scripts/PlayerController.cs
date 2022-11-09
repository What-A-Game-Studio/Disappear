using UnityEngine;
using Photon.Pun;
using WaG;
using WAG.Core.Controls;

[RequireComponent(
    typeof(Rigidbody),
    typeof(CapsuleCollider),
    typeof(StaminaController)
)]
public class PlayerController : MonoBehaviour
{
    private ModelInfos modelInfos;
    public static PlayerController MainPlayer { get; private set; }

    private Rigidbody rb;
    private StaminaController stamina;
    private Animator animator;
    private Vector3 currentVelocity;
    private float teamSpeedModifier = 0;
    public PlayerAnimationController Pac { get; private set; }

    public CrouchController CrouchController { get; private set; }
    public PlayerInventory PlayerInventory { get; private set; }
    public PhotonView Pv { get; private set; }
    private CameraController cameraController;


    [Header("Walk")] [SerializeField] private float walkSpeed = 2f;
    [Header("Run")] [SerializeField] private float runSpeedFactor = 0.5f;

    [Header("Weight Modifiers")] [SerializeField]
    private float lightOverweightSpeedModifier;

    [SerializeField] private float largeOverweightSpeedModifier;

    private bool grounded;
    private bool rpcGrounded;
    public bool Grounded => Pv.IsMine ? grounded : rpcGrounded;

    [Header("Jump")] [SerializeField] [Range(100, 1000)]
    private float jumpFactor = 260f;

    [SerializeField] private float airResistance = 0.8f;
    [SerializeField] private LayerMask groundCheck;

    [Header("Others")] [SerializeField] private float animBlendSpeed = 8.9f;
    [SerializeField] private float dis2Ground = 0.8f;

    [Header("OpenInventory")] [SerializeField]
    private GameObject gameUI;

    private bool inventoryStatus;
    private bool rpcInventoryStatus;
    public bool InventoryStatus => Pv.IsMine ? inventoryStatus : rpcInventoryStatus;

    private Vector3 rpcVelocity;

    public Vector3 PlayerVelocity => Pv.IsMine ? currentVelocity : rpcVelocity;
    public float? TemporarySpeedModifier { get; set; } = null;

    public Weight PlayerWeight { private get; set; }
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

        CrouchController = GetComponent<CrouchController>();
        if (CrouchController == null)
        {
            Debug.LogError("Need crouchController", this);
            Debug.Break();
        }

        if (!TryGetComponent<Rigidbody>(out rb))
        {
            Debug.LogError("Need Rigidbody", this);
            Debug.Break();
        }

        rb.freezeRotation = true;

        stamina = GetComponent<StaminaController>();

        Pv = GetComponent<PhotonView>();
        if (Pv == null)
        {
            Debug.LogError("Need PhotonView", this);
            Debug.Break();
        }


        InitModel();
        Pac = gameObject.AddComponent<PlayerAnimationController>();
        Pac.PC = this;


        if (!Pv.IsMine)
            return;

        Init();
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
        }

        Pv.RPC(nameof(RPC_Velocity), RpcTarget.All, currentVelocity);
        Pv.RPC(nameof(RPC_Ground), RpcTarget.All, Grounded);
    }

    //After FixedUpdate
    private void LateUpdate()
    {
        if (!Pv.IsMine)
            return;
    }

    #endregion Unity Events

    #region Privates

    private void InitModel()
    {
        // PlayerAnimationController pac = GetComponent<PlayerAnimationController>();
        name = PhotonNetwork.LocalPlayer.NickName;
        TeamController tc = GetComponent<TeamController>();
        modelInfos = tc.SetTeamData( Pv);

    }

    private void Init()
    {
        MainPlayer = this;
        cameraController = gameObject.AddComponent<CameraController>();
        cameraController.CameraRig = modelInfos.CameraRig;

        PlayerInventory = gameObject.AddComponent<PlayerInventory>();
        PlayerInventory.Init(gameUI, gameObject);

        InputManager.Instance.AddCallbackAction(
            ActionsControls.OpenInventory,
            (context) => HandleInventory()
        );
        InputManager.Instance.AddCallbackAction(
            ActionsControls.CloseInventory,
            (context) => HandleInventory()
        );
        InputManager.Instance.AddCallbackAction(
            ActionsControls.Interact,
            (context) => HandleInteract());
    }

    private void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void HandleInventory()
    {
        inventoryStatus = !inventoryStatus;
        Pv.RPC(nameof(RPC_InventoryStatus), RpcTarget.All, inventoryStatus);
    }

    private void HandleInteract()
    {
        Pv.RPC(nameof(RPC_Interact), RpcTarget.All);
    }

    private float targetSpeed;
    private void Move()
    {
        targetSpeed = walkSpeed;

        if (InputManager.Instance.Move == Vector2.zero)
        {
            targetSpeed = 0f;
        }
        
        if (InputManager.Instance.Run && (stamina.CanRun || DebuggerManager.Instance.UnlimitedStamina))
        {
            targetSpeed += targetSpeed * runSpeedFactor;
        }

        if (TemporarySpeedModifier.HasValue)
        {
            targetSpeed += targetSpeed * TemporarySpeedModifier.Value;
        }

        switch (PlayerWeight)
        {
            case Weight.LigthOverweight:
                targetSpeed += targetSpeed * lightOverweightSpeedModifier;
                break;
            case Weight.LargeOverweight:
                targetSpeed += targetSpeed * largeOverweightSpeedModifier;
                break;
            case Weight.Normal:
            default:
                break;
        }

        if (CrouchController.Crouched)
            targetSpeed += targetSpeed * CrouchController.CrouchSpeedFactor;

        if (grounded)
        {
            currentVelocity.x = Mathf.Lerp(currentVelocity.x,
                targetSpeed * InputManager.Instance.Move.x,
                animBlendSpeed * Time.fixedDeltaTime);
            currentVelocity.z = Mathf.Lerp(currentVelocity.z,
                targetSpeed * InputManager.Instance.Move.y,
                animBlendSpeed * Time.fixedDeltaTime);
            currentVelocity.y = 0;
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

    /// <summary>
    /// Check if player is on ground
    /// </summary>
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

    /// <summary>
    /// Set Speed by team
    /// </summary>
    /// <param name="teamDataSpeedModifier"></param>
    public void SetTeamSpeedModifier(float teamDataSpeedModifier)
    {
        teamSpeedModifier = teamDataSpeedModifier;
    }

    /// <summary>
    /// If the player controller is mine
    /// </summary>
    /// <returns></returns>
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
        Pac.InteractTrigger();
    }

    #endregion RPC
}