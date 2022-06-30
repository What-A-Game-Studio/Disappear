using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAnimationController : MonoBehaviour
{
    private Animator playerAnimator;
    private PlayerController pc;
    private bool isCrouching;
    private Vector3 velocity;
    private static readonly int IsCrouching = Animator.StringToHash("isCrouching");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");
    private static readonly int VelocityXHash = Animator.StringToHash("Velocity X");
    private static readonly int VelocityZHash = Animator.StringToHash("Velocity Z");
    private bool hasJumped = false;

    // Start is called before the first frame update
    private void Awake()
    {
        if (!TryGetComponent(out pc))
        {
           Debug.LogError("PlayerAnimationController required PlayerController", this);
           return;

        }
    }

    public void SetAnimator(Animator animator)
    {
        playerAnimator = animator;
    }

    // Update is called once per frame
    private void Update()
    {
        if(!playerAnimator)
            return;
            
        hasJumped = playerAnimator.GetBool(IsJumping);
        velocity = transform.InverseTransformVector(pc.PlayerVelocity);
        playerAnimator.SetFloat(VelocityXHash, velocity.x);
        playerAnimator.SetFloat(VelocityZHash, velocity.z);
        if (!hasJumped && !pc.Grounded)
            playerAnimator.SetBool(IsJumping, true);
        if (hasJumped && pc.Grounded)
            playerAnimator.SetBool(IsJumping, false);

        
    }
}