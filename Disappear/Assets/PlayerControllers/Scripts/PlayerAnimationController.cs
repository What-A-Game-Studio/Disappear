using UnityEngine;
public class PlayerAnimationController : MonoBehaviour
{        
    public static readonly int XVelHash = Animator.StringToHash("xVelocity");
    public static readonly int ZVelHash = Animator.StringToHash("zVelocity");
    public static readonly int YVelHash = Animator.StringToHash("yVelocity");
    public static readonly int FallingHash = Animator.StringToHash("Falling");
    public static readonly int GroundedHash = Animator.StringToHash("Grounded");
    public static readonly int JumpHash = Animator.StringToHash("Jump");
    public static readonly int CrouchHash = Animator.StringToHash("Crouch");
    public static readonly int InventoryHash = Animator.StringToHash("OpenInventory");
    public static readonly int Interact = Animator.StringToHash("Interact");
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
        animator.SetBool(PlayerAnimationController.CrouchHash, PC.Crouched);
        animator.SetBool(PlayerAnimationController.InventoryHash, PC.InventoryStatus);
        animator.SetBool(PlayerAnimationController.FallingHash, !PC.Grounded);
        animator.SetBool(PlayerAnimationController.GroundedHash, PC.Grounded);
    }

    public void InteractTrigger()
    {
        animator.SetTrigger(PlayerAnimationController.Interact);
    }
    
}