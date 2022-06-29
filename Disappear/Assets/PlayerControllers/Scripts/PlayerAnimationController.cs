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
    private static readonly int VelocityXHash = Animator.StringToHash("Velocity X");
    private static readonly int VelocityZHash = Animator.StringToHash("Velocity Z");
    
    // Start is called before the first frame update
    void Awake()
    {
        if (!TryGetComponent(out pc))
        {
           Debug.LogError("PlayerAnimationController required PlayerController", this);
           return;
        }
        
        if (!transform.GetChild(0).GetChild(0).TryGetComponent(out Animator pa))
        {
            Debug.LogError("PlayerAnimationController required Animator on mesh", this);
            return;
        }
        playerAnimator = pa;

    }

    public void SetAnimator(Animator animator)
    {
        playerAnimator = animator;
    }
    
    // Update is called once per frame
    void Update()
    {
        if(!playerAnimator)
            return;
        velocity = transform.InverseTransformVector(pc.PlayerVelocity);
        playerAnimator.SetFloat(VelocityXHash, velocity.x);
        playerAnimator.SetFloat(VelocityZHash, velocity.z);

    }
}
