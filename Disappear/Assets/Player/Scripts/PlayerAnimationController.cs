using UnityEngine;
using WAG.Health;

namespace WAG.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
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

        public PlayerController PC { private get; set; }

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
            animator.SetFloat(PlayerAnimationController.XVelHash, PC.PlayerVelocity.x);
            animator.SetFloat(PlayerAnimationController.ZVelHash, PC.PlayerVelocity.z);
            animator.SetFloat(PlayerAnimationController.YVelHash, PC.PlayerVelocity.y);
            animator.SetBool(PlayerAnimationController.CrouchHash, PC.CrouchController.Crouched);
            animator.SetBool(PlayerAnimationController.InventoryHash, PC.InventoryStatus);
            animator.SetBool(PlayerAnimationController.FallingHash, !PC.Grounded);
            animator.SetBool(PlayerAnimationController.GroundedHash, PC.Grounded);
            animator.SetBool(PlayerAnimationController.Wounded,
                PC.HealthController.CurrentHeathStatus != HeathStatus.Healthy);
        }

        public void InteractTrigger()
        {
            animator.SetTrigger(PlayerAnimationController.Interact);
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