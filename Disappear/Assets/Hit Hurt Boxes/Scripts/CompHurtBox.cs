using UnityEngine;
using WAG.HitHurtBoxes.Class;
using WAG.HitHurtBoxes.Interfaces;

namespace WAG.HitHurtBoxes
{
    public class CompHurtBox : MonoBehaviour, IHurtBox
    {
        [SerializeField] private bool active;
        [SerializeField] private Transform owner;

        public bool Active => active;
        public Transform Owner => owner;
        public Transform Transform => transform;

        public IHurtResponder HurtResponder { get; set; }

        public bool CheckHit(HitData data)
        {
            if (HurtResponder == null)
            {
                Debug.Log("No Responder", this);
            }

            return true;
        }
    }
}