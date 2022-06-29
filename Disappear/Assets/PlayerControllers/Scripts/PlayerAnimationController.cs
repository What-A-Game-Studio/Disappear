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
            throw new Exception("PlayerAnimationController required PlayerController");
        }

        if (!transform.GetChild(0).GetChild(0).TryGetComponent(out Animator pa))
        {
            throw new Exception("PlayerAnimationController required Animator on mesh");
        }

        playerAnimator = pa;
    }

    // Update is called once per frame
    private void Update()
    {
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