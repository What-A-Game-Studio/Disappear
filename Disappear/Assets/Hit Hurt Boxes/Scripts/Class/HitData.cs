using UnityEngine;
using WAG.HitHurtBoxes.Interfaces;

namespace WAG.HitHurtBoxes.Class
{
    /// <summary>
    /// Data sent during a hit
    /// </summary>
    public class HitData
    {
        public int Damage;
        public string ColliderName;
        public Vector3 HitPoint;
        public Vector3 HitNormal;
        public IHurtBox HurtBox;
        public IHitDetector HitDetector;

        /// <summary>
        /// Check if hit is valid
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            //This structure allows each "CheckHit" to modify the data,
            //which makes the system flexible
            if (HurtBox != null && HurtBox.CheckHit(this))
                if (HurtBox.HurtResponder == null || HurtBox.HurtResponder.CheckHit(this))
                    if (HitDetector.HitResponder == null || HitDetector.HitResponder.CheckHit(this))
                        return true;

            return false;
        }
    }
}