using UnityEngine;

namespace WAG.Audio.Footsteps
{
    public class FootstepEvent : MonoBehaviour
    {
        [Header("Footstep Sound Parameters")] [SerializeField]
        private float pitchVariation;

        [SerializeField] private SurfaceTypeSO defaultSurfaceType;
        [SerializeField] private LayerMask mask;
        [SerializeField] private GameObject footprintsPrefab;
        private Quaternion decalRotation;

        private void OnTriggerEnter(Collider other)
        {
            // if ((mask.value & (1 << other.gameObject.layer)) > 0
            //     && other.transform.gameObject.TryGetComponent(out SurfaceController sc))
            //     PlayFootstepSound(sc.SurfaceType);
            // else
            //     PlayFootstepSound(defaultSurfaceType);
        }

        private void PlayFootstepSound(SurfaceTypeSO surfaceType)
        {
            AudioClip clip = true
                ? surfaceType.Footstep.Walk
                : surfaceType.Footstep.Run;

            AudioManager.Instance.PlaySpatializeSoundOnce(clip,
                transform.position,
                1 + Random.Range(-pitchVariation, pitchVariation));

            if (surfaceType && surfaceType.PrintsOnSurface)
            {
                decalRotation.eulerAngles = new Vector3(90, transform.root.rotation.eulerAngles.y, 0);
                Instantiate(footprintsPrefab, transform.position, decalRotation);
            }
        }
    }
}