using UnityEngine;
using WAG.HitHurtBoxes.Class;

namespace WAG.HitHurtBoxes.Interfaces
{
    public interface IHurtBox
    {
        bool Active { get; }
        Transform Owner { get; }
        Transform Transform { get; }
        IHurtResponder HurtResponder { get; set; }

        /// <summary>
        /// Check if hurt box is "hurt"
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Can hit == true</returns>
        bool CheckHit(HitData data);
    }
}