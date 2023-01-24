using UnityEngine;
using WAG.Health;
using WAG.Multiplayer;
using WAG.Player.Health;

namespace WAG.Player
{
    public class NGOPlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private NGOPlayerHealthController healthController;
        [SerializeField] private NGOPlayerSync sync;

        public static readonly int XVelHash = Animator.StringToHash("xVelocity");
        public static readonly int ZVelHash = Animator.StringToHash("zVelocity");
        public static readonly int YVelHash = Animator.StringToHash("yVelocity");
        public static readonly int FallingHash = Animator.StringToHash("Falling");
        public static readonly int GroundedHash = Animator.StringToHash("Grounded");
        public static readonly int JumpHash = Animator.StringToHash("Jump");
        public static readonly int CrouchHash = Animator.StringToHash("Crouch");
        public static readonly int InventoryHash = Animator.StringToHash("Inventory");
        public static readonly int Interact = Animator.StringToHash("Interact");
        public static readonly int Wounded = Animator.StringToHash("Wounded");
        public static readonly int Attacking = Animator.StringToHash("Attacking");
        private Animator animator;


        // Start is called before the first frame update
        private void Awake()
        {
            if (!TryGetComponent<Animator>(out animator))
            {
                Debug.LogError("Need animator", this);
                Debug.Break();
            }
        }

        private void Start()
        {
            animator.Rebind();
        }

        // Update is called once per frame
        private void Update()
        {
            animator.SetFloat(NGOPlayerAnimationController.XVelHash, sync.RPCVelocity.Value.x);
            animator.SetFloat(NGOPlayerAnimationController.ZVelHash, sync.RPCVelocity.Value.z);
            animator.SetFloat(NGOPlayerAnimationController.YVelHash, sync.RPCVelocity.Value.y);
            animator.SetBool(NGOPlayerAnimationController.CrouchHash, sync.RPCCrouch.Value);
            animator.SetBool(NGOPlayerAnimationController.InventoryHash, sync.RPCInventoryStatus.Value);
            animator.SetBool(NGOPlayerAnimationController.FallingHash, !sync.RPCGrounded.Value);
            animator.SetBool(NGOPlayerAnimationController.GroundedHash, sync.RPCGrounded.Value);
            animator.SetBool(NGOPlayerAnimationController.Wounded,
                healthController.CurrentHeathStatus != HeathStatus.Healthy);
        }

        public void InteractTrigger()
        {
            animator.SetTrigger(NGOPlayerAnimationController.Interact);
        }

        public void Trigger(int animatorHash)
        {
            animator.SetTrigger(animatorHash);
        }

        public void ResetTrigger(int animatorHash)
        {
            animator.ResetTrigger(animatorHash);
        }
    }
}