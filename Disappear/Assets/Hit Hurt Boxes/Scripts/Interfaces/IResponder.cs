using UnityEngine;
using WAG.HitHurtBoxes.Class;

namespace WAG.HitHurtBoxes.Interfaces
{
    public interface IResponder
    {
        Transform Owner { get; }
        /// <summary>
        /// Check if hurt with data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool CheckHit(HitData data);

        /// <summary>
        /// When is "touch"
        /// May can reduce, increase damage, activate VFX ...  
        /// </summary>
        /// <param name="data"></param>
        void Response(HitData data);
    }
}