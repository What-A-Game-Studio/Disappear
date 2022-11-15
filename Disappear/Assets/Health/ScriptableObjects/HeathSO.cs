using UnityEngine;

namespace WAG.Health
{
    [CreateAssetMenu(fileName = "HeathSO", menuName = "SO/HeathSO")]
    public class HeathSO : ScriptableObject
    {
        [field:SerializeField] public HeathStatus Status { get; private set; }
        [field:SerializeField] public float SpeedModifier { get; private set; }
    }
}