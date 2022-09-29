using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName="New TeamDataSO",menuName="SO/TeamData")]
public class TeamData : ScriptableObject
{
 
    [field:SerializeField]
    public string Name { get; protected set; }

    
    [field:SerializeField]
    public VolumeProfile PostProcessingVolume { get; protected set; }

    [field:SerializeField]
    public GameObject Model { get; protected set; }
    
    [field: SerializeField] 
    public Vector3 ModelOffset { get; protected set; } = new Vector3(.0f,-0.85f,.0f);

    [field:SerializeField]
    public RuntimeAnimatorController AnimatorController { get; protected set; }
    
    [field:SerializeField]
    public float SpeedModifier { get; protected set; } = 1.0f;


}