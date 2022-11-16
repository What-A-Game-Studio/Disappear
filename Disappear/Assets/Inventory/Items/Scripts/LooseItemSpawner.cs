using UnityEngine;

namespace WAG.Inventory.Items
{
    public class LooseItemSpawner : ItemSpawner
    {
        [Range(1, 3)] [SerializeField] private float spawnRadius = 2;

        [Range(1, 10)] [SerializeField] private int maxItemsToSpawn = 1;
        protected int? nbToSpawn = null;

        public override int GetNbItemToSpawn()
        {
            if (nbToSpawn.HasValue)
                return nbToSpawn.Value;

            nbToSpawn = Random.Range(1, maxItemsToSpawn);
            return nbToSpawn.Value;
        }

        public override Vector3 SpawnCoordinate()
        {
            Vector2 pos = Random.insideUnitCircle * spawnRadius;

            return new Vector3(pos.x, 0, pos.y) + transform.position;
        }

    }
}