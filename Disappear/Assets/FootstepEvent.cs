using System.Collections.Generic;
using UnityEngine;

public class FootstepEvent : MonoBehaviour
{
    [Header("Footstep Sounds")] [SerializeField]
    private List<AudioClip> walkClip;

    [SerializeField] private float pitchVariation;

    private Transform feet;

    private void Awake()
    {
        feet = transform.parent.GetChild(transform.GetSiblingIndex() + 1);
    }

    public void PlayFootstepSound()
    {
        AudioManager.Instance.PlaySpatializedSoundOnce(walkClip[Random.Range(0, walkClip.Count)], feet.position,
            1 + Random.Range(-pitchVariation, pitchVariation));
    }
}