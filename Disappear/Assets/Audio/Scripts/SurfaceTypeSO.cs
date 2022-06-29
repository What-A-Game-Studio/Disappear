using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SurfaceType
{
    Concret,
    Dirt,
    Snow,
    Water
}

[CreateAssetMenu(fileName = "New Surface", menuName = "SO/Surface")]
public class SurfaceTypeSO : ScriptableObject
{
    [field: SerializeField] public SurfaceType Type { get; private set; }

    [field: SerializeField] public List<AudioClip> FootstepOnSurface { get; private set; }
    // [field: SerializeField] public List<AudioClip> JumpOnSurface { get; private set; }
    // [field: SerializeField] public List<AudioClip> LandOnSurface { get; private set; }
}