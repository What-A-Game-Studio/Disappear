using UnityEngine;

[CreateAssetMenu(fileName="New Item",menuName="SO/Item")]
public class ItemData : ScriptableObject
{
    [field: SerializeField] public string ShortName { get; set; }
    [field: SerializeField] public string FullName { get; set; }
    [field: SerializeField] public string Description { get; set; }
    [field: SerializeField] public int Value { get; set; }
    [field: SerializeField] public float Weight { get; set; }
    [field: SerializeField] public GameObject Model { get; set; }
    [field: SerializeField] public Texture2D Image { get; set; }
}
