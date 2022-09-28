using System.Collections.Generic;
using Audio.Footstep.Scriptable_Object;
using UnityEngine;

namespace Audio.Scripts
{
    [CreateAssetMenu(fileName = "SurfaceData", menuName = "SO/Surface")]
    public class SurfaceTypeSO : ScriptableObject
    {
        [field: SerializeField] public SurfaceType Type { get; private set; }
        [field: SerializeField] public FootstepSoundData Footstep { get; private set; }
        [field: SerializeField] public bool PrintsOnSurface { get; private set; }
    }
}