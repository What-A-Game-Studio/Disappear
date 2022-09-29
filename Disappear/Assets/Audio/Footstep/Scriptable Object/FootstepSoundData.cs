using System.Collections.Generic;
using UnityEngine;

namespace Audio.Footstep.Scriptable_Object
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "SO/Sound Data")]
    public class FootstepSoundData : ScriptableObject
    {
        [SerializeField] private AudioClip defaultClip;
        [SerializeField] private List<AudioClip> walks;
        [SerializeField] private List<AudioClip> runs;
        [SerializeField] private List<AudioClip> lands;

        public AudioClip Walk => walks.Count > 0 ? walks[Random.Range(0, walks.Count)] : defaultClip;
        public AudioClip Run => runs.Count > 0 ? runs[Random.Range(0, runs.Count)] : defaultClip;
        public AudioClip Land => lands.Count > 0 ? lands[Random.Range(0, lands.Count)] : defaultClip;
    }
}