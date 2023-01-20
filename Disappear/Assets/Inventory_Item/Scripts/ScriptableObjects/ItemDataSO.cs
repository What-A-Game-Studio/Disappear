using System;
using UnityEditor;
using UnityEngine;
using WAG.Core;

namespace WAG.Inventory_Items
{

    [CreateAssetMenu(fileName = "New Item", menuName = "SO/Item")]
    public class ItemDataSO : ScriptableObject
    {
        [field: ScriptableObjectId]
        [field: SerializeField] public string Uid { get; private set; }
        [field: SerializeField] public string ShortName { get; private set; }
        [field: SerializeField] public string FullName { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: SerializeField] public float Weight { get; private set; } = 1f;
        [field: SerializeField] public Vector2Int Size { get; private set; } = new Vector2Int(1, 1);
        [field: SerializeField] public ItemType ItemType { get; private set; }

        [field: SerializeField] public RarityTiersEnum TierEnum { get; private set; }

        [field: SerializeField] public GameObject Model { get; private set; }
        [field: SerializeField] public Sprite Image { get; private set; }


        //Item value =  (Size.X * Size.Y / Tier) * 100 
        public int Value => Size.x * Size.y * (int) TierEnum * 100;
    }
}