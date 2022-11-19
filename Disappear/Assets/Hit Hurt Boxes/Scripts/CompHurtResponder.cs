using UnityEngine;
using System.Collections.Generic;
using WAG.HitHurtBoxes.Class;
using WAG.HitHurtBoxes.Interfaces;

namespace WAG.HitHurtBoxes
{
    public class CompHurtResponder : MonoBehaviour, IHurtResponder
    {
        private List<IHurtBox> hurtBoxes = new List<IHurtBox>();

        private void Start()
        {
            //Get all Hurt boxes & set HurtResponder to this
            hurtBoxes = new List<IHurtBox>(GetComponentsInChildren<IHurtBox>());
            for (int i = 0; i < hurtBoxes.Count; i++)
                hurtBoxes[i].HurtResponder = this;
        }

        public bool CheckHit(HitData data)
        {
            return true;
        }

        public void Response(HitData data)
        {
            Debug.Log(data.ColliderName + " take " + data.Damage + " damage(s)", this);
        }
    }
}