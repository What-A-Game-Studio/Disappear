using System;
using System.Linq;
using UnityEngine;
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
        typeof(NGOTeamController),
        typeof(NGOCrouchController),
        typeof(NGOPlayerHealthController)
    )]
    [RequireComponent(
        typeof(NGOPlayerSpeedController),
        typeof(NGOPlayerAnimationController),
        typeof(NGOPlayerSync)
    )]
    public class NGOPlayerController : NetworkSideBehaviour
    {
        public static NGOPlayerController MainPlayer { get; private set; }

        private float maxWeight = 50f;
        private ModelInfos modelInfos;

        private Vector3 currentVelocity;
        public bool CanRotate { get; set; } = true;
        public bool CanMove { get; set; } = true;

        private bool grounded;
        public bool Grounded => sync.IsMine ? grounded : sync.RPCGrounded.Value;

        [Header("Jump")] [SerializeField] [Range(100, 1000)]
        private float jumpFactor = 260f;


        [SerializeField] private float airResistance = 0.8f;
        [SerializeField] private LayerMask groundCheck;

        [Header("Others")] [SerializeField] private float animBlendSpeed = 8.9f;
        [SerializeField] private float dis2Ground = 0.8f;

        [Header("OpenInventory")] [SerializeField]
        private GameObject inventoryUI;

        private bool inventoryStatus;
        public bool InventoryStatus => sync.IsMine ? inventoryStatus : sync.RPCInventoryStatus.Value;


        public Vector3 PlayerVelocity => sync.IsMine ? currentVelocity : sync.RPCVelocity.Value;


        #region Needed Components

        private Rigidbody rb;
        private Animator animator;

        public NGOPlayerHealthController HealthController => healthController;
        private NGOPlayerHealthController healthController;
        private NGOPlayerAnimationController pac;
        public InventoryController InventoryController { get; private set; }
        private CameraController cameraController;

        private NGOPlayerSpeedController speedController;
        public NGOPlayerSpeedController SpeedController => speedController;

        private NGOPlayerSync sync;
        public NGOPlayerSync Sync => sync;

        private StaminaController stamina;
        private NGOCrouchController crouchController;

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


            if (!TryGetComponent<NGOPlayerSpeedController>(out speedController))
            {
                Debug.LogError("Need NGOPlayerSpeedController", this);
                Debug.Break();
            }

            if (!TryGetComponent<NGOPlayerAnimationController>(out pac))
            {
                Debug.LogError("Need NGOPlayerAnimationController", this);
                Debug.Break();
            }
            else
                pac.PC = this;

            if (!TryGetComponent<NGOCrouchController>(out crouchController))
            {
                Debug.LogError("Need NGOCrouchController", this);
                Debug.Break();
            }
            else
            {
                crouchController.PlayerController = this;
            }

            if (!TryGetComponent<NGOPlayerHealthController>(out healthController))
            {
                Debug.LogError("Need PlayerHealthController", this);
                Debug.Break();
            }

            if (!TryGetComponent<NGOPlayerSync>(out sync))
            {
                Debug.LogError("Need PlayerSync", this);
                Debug.Break();
            }
            else
            {
                sync.Init((NGOHealthStatusController)healthController, () => { pac.InteractTrigger(); });
            }
        }

        #endregion Needed Components

        #region Unity Events

        // public override void OnNetworkSpawn()
        // {
        //     Debug.Log("Check Components");
        //     InputManager.Instance.SwitchMap(ControlMap.Player);
        //     HideCursor();
        //     GetNeededComponents();
        //     InitModel();
        //
        //     if (!sync.IsMine)
        //         return;
        //
        //     Init();
        // }

        protected override void OnClientSpawn()
        {
            if (!IsLocalPlayer) return;

            InputManager.Instance.SwitchMap(ControlMap.Player);
            HideCursor();
            GetNeededComponents();
            sync.SyncName(NGOMultiplayerManager.Instance.localPlayer.playerName);
            InitModel();
            Init();
        }

        protected override void OnServerSpawn()
        {
            if (!IsLocalPlayer) return;

            GetNeededComponents();
            this.name = LobbyManager.Instance.CurrentLobby.Players.FirstOrDefault(player =>
                    player.Id == NGOMultiplayerManager.Instance.localPlayer.playerId)
                ?.Data["PlayerName"].Value ?? "Player";
            InitModel();
        }
        
        protected override void UpdateServer()
        {
            //throw new System.NotImplementedException();
        }

        protected override void UpdateClient()
        {
            // sync.SyncMove(new Vector2(InputManager.Instance.Move.x, InputManager.Instance.Move.y));
        }

        protected override void FixedUpdateServer()
        {
            //  throw new System.NotImplementedException();
        }

        protected override void FixedUpdateClient()
        {
            //   throw new System.NotImplementedException();
        }

        private void FixedUpdate()
        {
        /*    SampleGround();
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
            sync.SyncVelocity(currentVelocity);*/
        }

        #endregion Unity Events

        #region Privates

        private void InitModel()
        {
            NGOTeamController tc = GetComponent<NGOTeamController>();
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
                    targetSpeed * sync.RPCMove.Value.x,
                    animBlendSpeed * Time.fixedDeltaTime);

                currentVelocity.z = Mathf.Lerp(currentVelocity.z,
                    targetSpeed * sync.RPCMove.Value.y,
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
            animator.SetTrigger(NGOPlayerAnimationController.JumpHash);
        }


        /// <summary>
        /// Add force for jump, Used in Animation Event
        /// </summary>
        public void JumpAddForce()
        {
            rb.AddForce(-rb.velocity.y * Vector3.up, ForceMode.VelocityChange);
            rb.AddForce(Vector3.up * jumpFactor, ForceMode.Impulse);
            animator.ResetTrigger(NGOPlayerAnimationController.JumpHash);
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