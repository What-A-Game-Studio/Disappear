using UnityEngine;
using Photon.Pun;
using WAG.Core.Controls;
using WAG.Health;
using WAG.Inventory;
using WAG.Multiplayer;
using WAG.Player.Enums;
using WAG.Player.Health;
using WAG.Player.Models;
using WAG.Player.Movements;
using WAG.Player.Teams;

namespace WAG.Player
{
    [RequireComponent(
        typeof(Animator),
        typeof(Rigidbody),
        typeof(CapsuleCollider)
    )]
    [RequireComponent(
        typeof(TeamController),
        typeof(CrouchController),
        typeof(PlayerHealthController)
    )]
    [RequireComponent(
        typeof(PlayerSpeedController),
        typeof(PlayerAnimationController),
        typeof(PlayerSync)
    )]
    public class PlayerController : MonoBehaviour
    {
        private float maxWeight = 50f;
        private ModelInfos modelInfos;
        public static PlayerController MainPlayer { get; private set; }

        private Rigidbody rb;
        private Vector3 currentVelocity;
        public bool CanRotate { get; set; } = true;
        public bool CanMove { get; set; } = true;

        private bool grounded;
        public bool Grounded => sync.IsMine ? grounded : sync.RPCGrounded;

        [Header("Jump")] [SerializeField] [Range(100, 1000)]
        private float jumpFactor = 260f;


        [SerializeField] private float airResistance = 0.8f;
        [SerializeField] private LayerMask groundCheck;

        [Header("Others")] [SerializeField] private float animBlendSpeed = 8.9f;
        [SerializeField] private float dis2Ground = 0.8f;

        [Header("OpenInventory")] [SerializeField]
        private GameObject inventoryUI;

        private bool inventoryStatus;
        public bool InventoryStatus => sync.IsMine ? inventoryStatus : sync.RPCInventoryStatus;


        public Vector3 PlayerVelocity => sync.IsMine ? currentVelocity : sync.RPCVelocity;


        #region Needed Components

        private Animator animator;

        public PlayerHealthController HealthController => healthController;
        private PlayerHealthController healthController;
        private PlayerAnimationController pac;
        public InventoryController InventoryController { get; private set; }
        private CameraController cameraController;

        private PlayerSpeedController speedController;
        public PlayerSpeedController SpeedController => speedController;

        private PlayerSync sync;
        public PlayerSync Sync => sync;

        private StaminaController stamina;
        private CrouchController crouchController;

        private void GetNeededComponents()
        {
            if (!TryGetComponent<Animator>(out animator))
            {
                Debug.LogError("Need animator", this);
                Debug.Break();
            }

            if (!TryGetComponent<Rigidbody>(out rb))
            {
                Debug.LogError("Need Rigidbody", this);
                Debug.Break();
            }

            rb.freezeRotation = true;


            if (!TryGetComponent<PlayerSpeedController>(out speedController))
            {
                Debug.LogError("Need PlayerSpeedController", this);
                Debug.Break();
            }

            if (!TryGetComponent<PlayerAnimationController>(out pac))
            {
                Debug.LogError("Need PlayerAnimationController", this);
                Debug.Break();
            }
            else
                pac.PC = this;

            if (!TryGetComponent<CrouchController>(out crouchController))
            {
                Debug.LogError("Need CrouchController", this);
                Debug.Break();
            }
            else
            {
                crouchController.PlayerController = this;
            }

            if (!TryGetComponent<PlayerHealthController>(out healthController))
            {
                Debug.LogError("Need PlayerHealthController", this);
                Debug.Break();
            }

            if (!TryGetComponent<PlayerSync>(out sync))
            {
                Debug.LogError("Need PlayerSync", this);
                Debug.Break();
            }
            else
            {
                sync.Init((HealthStatusController) healthController, () => { pac.InteractTrigger(); });
            }
        }

        #endregion Needed Components

        #region Unity Events

        void Awake()
        {
            InputManager.Instance.SwitchMap(ControlMap.Player);
            HideCursor();
            GetNeededComponents();
            InitModel();

            if (!sync.IsMine)
                return;

            Init();
        }

        private void FixedUpdate()
        {
            SampleGround();
            if (!sync.IsMine)
                return;

            if (CanMove)
            {
                Move();
                HandleJump();
            }
            else
            {
                currentVelocity = Vector3.zero;
            }

            sync.SyncGround(grounded);
            sync.SyncVelocity(currentVelocity);
        }

        #endregion Unity Events

        #region Privates

        private void InitModel()
        {
            name = PhotonNetwork.LocalPlayer.NickName;
            TeamController tc = GetComponent<TeamController>();
            modelInfos = tc.SetTeamData(sync.IsSeeker(), this);
        }

        private void Init()
        {
            MainPlayer = this;
            cameraController = gameObject.AddComponent<CameraController>();
            cameraController.CameraRig = modelInfos.CameraRig;

            InventoryController = gameObject.AddComponent<InventoryController>();
            InventoryController.Init(inventoryUI, gameObject, modelInfos.ObjectHolder);
            InventoryController.OnChangeWeight += currentWeight =>
            {
                if (currentWeight > maxWeight * 0.75f)
                    speedController.PlayerWeight = Weight.LargeOverweight;

                else if (currentWeight > maxWeight * 0.5f)
                    speedController.PlayerWeight = Weight.LightOverweight;
                else
                    speedController.PlayerWeight = Weight.Normal;
            };
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
                (context) => sync.HandleInteract());
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void HandleInventory()
        {
            inventoryStatus = !inventoryStatus;
            sync.SyncInventoryStatus(inventoryStatus);
        }

        private void Move()
        {
            float targetSpeed = speedController.GetSpeed();

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
            speedController.SetTeamSpeedModifier(teamDataSpeedModifier);
        }


        /// <summary>
        /// If the player controller is mine
        /// </summary>
        /// <returns></returns>
        public bool IsMine()
        {
            return sync.IsMine;
        }

        #endregion Public

        #region RPC

        #endregion RPC
    }
}