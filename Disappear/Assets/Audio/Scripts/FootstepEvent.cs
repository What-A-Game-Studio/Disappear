using System.Collections.Generic;
using UnityEngine;

public class FootstepEvent : MonoBehaviour
{
    [Header("Footstep Sound Parameters")] [SerializeField]
    private float pitchVariation;

    [SerializeField] private AudioClip defaultFootStep;
    private SurfaceTypeSO surfaceType;
    public bool IsPlaying { get; set; } = false;
    private Transform feet;

    [SerializeField] private GameObject footprintsPrefab;
    private Quaternion decalRotation;

    private void Awake()
    {
    }

    public void Init(Transform f)
    {
        feet = f;
    }

    public void PlayFootstepSound()
    {
        if (!IsPlaying)
        {
            IsPlaying = true;
            AudioClip toPlay = (surfaceType && surfaceType.FootstepOnSurface.Count > 0)
                ? surfaceType.FootstepOnSurface[Random.Range(0, surfaceType.FootstepOnSurface.Count)]
                : defaultFootStep;

            AudioManager.Instance.PlaySpatializeSoundOnce(this,
                toPlay, feet.position,
                1 + Random.Range(-pitchVariation, pitchVariation));
            decalRotation.eulerAngles = new Vector3(90, transform.root.rotation.eulerAngles.y, 0);
            Instantiate(footprintsPrefab, feet.position, decalRotation);
        }
    }

    public void ChangeSurfaceType(SurfaceTypeSO st)
    {
        if (surfaceType == null || st.Type != surfaceType.Type)
        {
            surfaceType = st;
        }
    }
}