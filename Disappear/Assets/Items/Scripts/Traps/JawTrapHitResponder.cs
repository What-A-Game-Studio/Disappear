using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAG.HitHurtBoxes;
using WAG.HitHurtBoxes.Class;

namespace WAG.Items
{
    public class JawTrapHitResponder : CompHitResponder
    {
        [SerializeField] protected bool canAttack;
        public static readonly int Open = Animator.StringToHash("Open");
        [SerializeField] private Animator animator;

        protected void Update()
        {
            if (canAttack && hitBox.CheckHit())
                canAttack = false;
        }
        
        public override void Response(HitData data)
        {
            animator.SetBool(Open,false);
            Debug.Log("JawTrapTrigger", this);
        }
    }
}
