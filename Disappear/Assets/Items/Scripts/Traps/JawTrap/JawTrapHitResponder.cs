using UnityEngine;
using UnityEngine.InputSystem;
using WAG.Core.Controls;
using WAG.HitHurtBoxes;
using WAG.HitHurtBoxes.Class;
using WAG.Player;

namespace WAG.Items
{
    public class JawTrapHitResponder : CompHitResponder
    {
        [SerializeField] protected bool canAttack;
        public static readonly int Open = Animator.StringToHash("Open");
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject trapUI;

        protected void Update()
        {
            if (canAttack && hitBox.CheckHit(out HitData data))
                canAttack = false;
        }

        public override bool CheckHit(HitData data)
        {
            return true;
        }

        public override void Response(HitData data)
        {
            canAttack = false;
            animator.SetBool(Open,false);
            if (data.HurtBox.Owner.parent.TryGetComponent<PlayerController>(out PlayerController pc))
            {
                pc.CanMove = false;
                Vector3 pos = new Vector3(transform.position.x, pc.transform.position.y, transform.position.z);
                pc.transform.position = pos;
                GameObject trapui = Instantiate(trapUI);
                TrapUIController tuic = trapui.GetComponent<TrapUIController>();
                tuic.PlayerController = pc;
            }
           
            Debug.Log("JawTrapTrigger", this);
        }


    }
}
