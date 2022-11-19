using UnityEngine;
using WAG.Inventory_Items;

namespace WAG.Items
{
    [CreateAssetMenu(fileName = "New Rarity Tiers", menuName = "SO/Rarity Tier")]
    public class RarityTierSO : ScriptableObject
    {
        [field: SerializeField] public RarityTiersEnum Rarity { get; protected set; }
        [field: SerializeField] public int Rate { get; protected set; }
        [field: SerializeField] public float ValueMultiplier { get; protected set; }

    }
}