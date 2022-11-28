using WAG.HitHurtBoxes.Class;

namespace WAG.HitHurtBoxes.Interfaces
{
    public interface IHitDetector
    {
        IHitResponder HitResponder { get; set; }
        /// <summary>
        /// Search for hurt boxes and execute responses of HitBoxResponder & HurtBoxResponder
        /// </summary>
        /// <returns>True if find one </returns>
        bool CheckHit(out HitData data);
    }
}