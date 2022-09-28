using System;
using Audio.Footstep.Scriptable_Object;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio.Scripts.Footsteps
{
    public class FootstepEvent : MonoBehaviour
    {
        [Header("Footstep Sound Parameters")] [SerializeField]
        private float pitchVariation;

        [SerializeField] private SurfaceTypeSO defaultSurfaceType;
        [SerializeField] private float maxRayDistance = 0.1f;
        [SerializeField] private LayerMask mask;
        public bool IsPlaying { get; set; } = true;
        private Transform feet;
        [SerializeField] private GameObject footprintsPrefab;
        private Quaternion decalRotation;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.gameObject.TryGetComponent(out SurfaceController sc))
                PlayFootstepSound(sc.SurfaceType);
            else
                PlayFootstepSound(defaultSurfaceType);
        }

        // private void OnTriggerEnter()
        // {
        //     Debug.DrawRay(transform.position,Vector3.down*maxRayDistance);
        //     if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxRayDistance, mask))
        //     {
        //         if (canPlaySound)
        //         {
        //             
        //         }
        //     }
        //     else
        //     {
        //         canPlaySound = true;
        //     }
        //     
        // }
        private void PlayFootstepSound(SurfaceTypeSO surfaceType)
        {
            IsPlaying = true;
            AudioClip toPlay = true
                ? surfaceType.Footstep.Walk
                : surfaceType.Footstep.Run;

            AudioManager.Instance.PlaySpatializeSoundOnce(this,
                toPlay, transform.position,
                1 + Random.Range(-pitchVariation, pitchVariation));

            if (surfaceType && surfaceType.PrintsOnSurface)
            {
                decalRotation.eulerAngles = new Vector3(90, transform.root.rotation.eulerAngles.y, 0);
                GameObject fpGO = Instantiate(footprintsPrefab, feet.position, decalRotation);
            }
        }
    }
}