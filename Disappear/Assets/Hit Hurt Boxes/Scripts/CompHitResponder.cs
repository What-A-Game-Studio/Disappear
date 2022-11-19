using UnityEngine;
using WAG.HitHurtBoxes.Class;
using WAG.HitHurtBoxes.Interfaces;

namespace WAG.HitHurtBoxes
{
    public class CompHitResponder : MonoBehaviour, IHitResponder
    {
        [SerializeField] protected int damage = 1;
        [SerializeField] protected CompHitBox hitBox;
        public int Damage => damage;

        protected virtual void Awake()
        {
            hitBox.HitResponder = this;
        }

        // protected virtual void Update()
        // {
        //     if (canAttack && hitBox.CheckHit())
        //         canAttack = false;
        // }
        
        public virtual bool CheckHit(HitData data)
        {
            return true;
        }

        public virtual void Response(HitData data)
        {
        }
    }
}