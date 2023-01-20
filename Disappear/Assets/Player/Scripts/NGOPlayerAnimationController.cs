using UnityEngine;
using WAG.Health;

namespace WAG.Player
{
    public class NGOPlayerAnimationController : MonoBehaviour
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

        public NGOPlayerController PC { private get; set; }

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
            animator.SetFloat(NGOPlayerAnimationController.XVelHash, PC.PlayerVelocity.x);
            animator.SetFloat(NGOPlayerAnimationController.ZVelHash, PC.PlayerVelocity.z);
            animator.SetFloat(NGOPlayerAnimationController.YVelHash, PC.PlayerVelocity.y);
            animator.SetBool(NGOPlayerAnimationController.CrouchHash, PC.SpeedController.CrouchController.Crouched);
            animator.SetBool(NGOPlayerAnimationController.InventoryHash, PC.InventoryStatus);
            animator.SetBool(NGOPlayerAnimationController.FallingHash, !PC.Grounded);
            animator.SetBool(NGOPlayerAnimationController.GroundedHash, PC.Grounded);
            animator.SetBool(NGOPlayerAnimationController.Wounded,
                PC.HealthController.CurrentHeathStatus != HeathStatus.Healthy);
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