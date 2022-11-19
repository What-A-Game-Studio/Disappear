using UnityEngine;
using WAG.HitHurtBoxes.Class;
using WAG.HitHurtBoxes.Interfaces;

namespace WAG.HitHurtBoxes
{
    public class CompHitBox : MonoBehaviour, IHitDetector
    {
        [SerializeField] private BoxCollider collider;
        [SerializeField] private LayerMask layer;
        [SerializeField] private float thickness = 0.025f;

        public IHitResponder HitResponder { get; set; }


        public bool CheckHit()
        {
            Vector3 scaledSize = new Vector3(
                collider.size.x * transform.lossyScale.x,
                collider.size.y * transform.lossyScale.y,
                collider.size.z * transform.lossyScale.z
            );

            float distance = scaledSize.y - thickness;
            Vector3 direction = transform.up;
            Vector3 center = transform.TransformPoint(collider.center);
            Vector3 start = center - direction * (distance / 2);
            Vector3 halfExtents = new Vector3(scaledSize.x, thickness, scaledSize.z) / 2;

            Quaternion orientation = transform.rotation;

            HitData hitData = null;
            IHurtBox hurtBox = null;
            RaycastHit[] hits = Physics.BoxCastAll(start, halfExtents, direction, orientation, layer);
            bool isValid = false;
            foreach (RaycastHit hit in hits)
            {
                //Search on collider because transform can try to get component in parent of this collider
                if (hit.collider.TryGetComponent<IHurtBox>(out hurtBox))
                {
                    if (hurtBox.Active)
                    {
                        hitData = new HitData
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

                        if (hitData.Validate())
                        {
                            //Call Response of they aren't null
                            hitData.HitDetector.HitResponder?.Response(hitData);
                            hitData.HurtBox.HurtResponder?.Response(hitData);
                            isValid = true;
                        }
                    }
                }
            }

            return isValid;
        }
    }
}