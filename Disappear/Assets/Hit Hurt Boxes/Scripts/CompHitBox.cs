using UnityEngine;
using WAG.HitHurtBoxes.Class;
using WAG.HitHurtBoxes.Interfaces;

namespace WAG.HitHurtBoxes
{
    public class CompHitBox : MonoBehaviour, IHitDetector
    {
        [SerializeField] private BoxCollider collider;
        [SerializeField] private LayerMask layer;
        public bool MultipleHit { get; set; } = true;
        private const float Thickness = 0.025f;

        public IHitResponder HitResponder { get; set; }


        public bool CheckHit(out HitData data)
        {
            Vector3 scaledSize = new Vector3(
                collider.size.x * transform.lossyScale.x,
                collider.size.y * transform.lossyScale.y,
                collider.size.z * transform.lossyScale.z
            );

            float distance = scaledSize.y - Thickness;
            Vector3 direction = transform.up;
            Vector3 center = transform.TransformPoint(collider.center);
            Vector3 start = center - direction * (distance / 2);
            Vector3 halfExtents = new Vector3(scaledSize.x, Thickness, scaledSize.z) / 2;

            Quaternion orientation = transform.rotation;

            IHurtBox hurtBox = null;
            RaycastHit[] hits = Physics.BoxCastAll(start, halfExtents, direction, orientation, layer);
            bool isValid = false;
            data = null;
            foreach (RaycastHit hit in hits)
            {
                //Search on collider because transform can try to get component in parent of this collider
                if (hit.collider.TryGetComponent<IHurtBox>(out hurtBox))
                {
                    if (hurtBox.Active)
                    {
                        data = new HitData
                        {
                            //Damage deal
                            Damage = HitResponder?.Damage ?? 0,
                            // HitPoint is either the center of the area or the point of contact of the rays.
                            // "hit.point == Vector3.zero ? center" if the cast start in a collider
                            HitPoint = hit.point == Vector3.zero ? center : hit.point,
                            //The normal of the surface the ray hit
                            HitNormal = hit.normal,
                            // Find on Collider
                            HurtBox = hurtBox,
                            HitDetector = this,
                            ColliderName = hit.collider.transform.name
                        };

                        //Can't hit him self 
                        if (hurtBox.Owner.GetInstanceID() != HitResponder.Owner.GetInstanceID() && data.Validate())
                        {
                            //Call Response of they aren't null
                            data.HitDetector.HitResponder?.Response(data);
                            data.HurtBox.HurtResponder?.Response(data);
                            isValid = true;

                            if (!MultipleHit)
                                return true;
                        }
                    }
                }
            }

            return isValid;
        }
    }
}