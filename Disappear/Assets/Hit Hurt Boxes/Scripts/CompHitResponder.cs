using UnityEngine;
using WAG.HitHurtBoxes.Class;
using WAG.HitHurtBoxes.Interfaces;

namespace WAG.HitHurtBoxes
{
    public class CompHitResponder : MonoBehaviour, IHitResponder
    {
        [SerializeField] protected int damage = 1;
        [SerializeField] protected CompHitBox hitBox;
        [SerializeField] protected bool multipleHit = true;
        [field: SerializeField] public Transform Owner { get; protected set; }
        public int Damage => damage;

        protected virtual void Awake()
        {
            hitBox.HitResponder = this;
            hitBox.MultipleHit = multipleHit;
        }

        // protected virtual void Update()
        // {
        //     if (canAttack && hitBox.CheckHit())
        //         canAttack = false;
        // }

        /// <summary>
        /// Conditions : 
        /// Make sure nothing stand between Hit & Hurt
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool CheckHit(HitData data)
        {
            Transform target = data.HurtBox.HurtResponder.Owner;
            Vector3 attackOrigin = transform.position;
            Ray r = new Ray(attackOrigin, target.position - attackOrigin);
             bool isSomethingBetween = (Physics.Raycast(r, out RaycastHit hitInfo) &&
                    target.gameObject.GetInstanceID() ==
                    hitInfo.collider.gameObject.GetInstanceID());
             
             return !isSomethingBetween;
        }

        public virtual void Response(HitData data)
        {
        }


        

    }
}