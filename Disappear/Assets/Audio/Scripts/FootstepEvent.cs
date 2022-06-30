using System.Collections.Generic;
using UnityEngine;

public class FootstepEvent : MonoBehaviour
{
    [Header("Footstep Sound Parameters")] 
    
    [SerializeField]
    private float pitchVariation;

    [SerializeField] private AudioClip defaultFootStep;
    private SurfaceTypeSO surfaceType;

    private Transform feet;

    private void Awake()
    {
     //   feet = transform.parent.GetChild(transform.GetSiblingIndex() + 1);
    }

    public void Init(Transform f)
    {
        feet = f;
    }

    public void PlayFootstepSound()
    {
        AudioClip toPlay = (surfaceType && surfaceType.FootstepOnSurface.Count > 0)
            ? surfaceType.FootstepOnSurface[Random.Range(0, surfaceType.FootstepOnSurface.Count)]
            : defaultFootStep;
        
        AudioManager.Instance.PlaySpatializedSoundOnce(
            toPlay, feet.position,
            1 + Random.Range(-pitchVariation, pitchVariation));
    }

    public void ChangeSurfaceType(SurfaceTypeSO st)
    {
        if (surfaceType == null || st.Type != surfaceType.Type)
        {
            surfaceType = st;
        }
    }
}