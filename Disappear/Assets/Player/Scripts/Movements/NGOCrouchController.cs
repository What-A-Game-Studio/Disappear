using Unity.Netcode;
using UnityEngine;
using WAG.Core.Controls;


namespace WAG.Player.Movements
{
    public class NGOCrouchController : NetworkBehaviour
    {
        [SerializeField] private float crouchSpeedFactor = -0.5f;
        public float CrouchSpeedFactor => Crouched ? crouchSpeedFactor : 0;

        [SerializeField] private float capsuleColliderStandRadius = .28f;
        [SerializeField] private float capsuleColliderStandHeight = 1.78f;

        [SerializeField] private float capsuleColliderCrouchRadius = .42f;
        [SerializeField] private float capsuleColliderCrouchHeight = .89f;
        private CapsuleCollider capsuleCollider;

        public bool Crouched =>
            PlayerController.IsMine ? InputManager.Instance.Crouch : PlayerController.Sync.RPCCrouch.Value;

        public NGOPlayerController PlayerController { get; set; }


        private void Awake()
        {
            if (!TryGetComponent<CapsuleCollider>(out capsuleCollider))
            {
                Debug.LogError("Need CapsuleCollider", this);
                Debug.Break();
            }

            capsuleCollider.height = capsuleColliderStandRadius;
            capsuleCollider.radius = capsuleColliderStandHeight;
        }


        // Update is called once per frame
        private void Update()
        {
            if (Crouched)
            {
                capsuleCollider.height = capsuleColliderCrouchHeight;
                capsuleCollider.radius = capsuleColliderCrouchRadius;
            }
            else
            {
                capsuleCollider.height = capsuleColliderStandHeight;
                capsuleCollider.radius = capsuleColliderStandRadius;
            }

            capsuleCollider.center = new Vector3(0f, capsuleCollider.height / 2, 0f);
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;
            PlayerController.Sync.SyncCrouch(Crouched);
        }
    }
}