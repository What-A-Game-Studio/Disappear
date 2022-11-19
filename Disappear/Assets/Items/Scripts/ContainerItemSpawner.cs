using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WAG.Items
{
    public class ContainerItemSpawner : ItemSpawner
    {
        [SerializeField] [Range(0, 1)] private float fillPercentage = 0.2f;
        private List<Vector3> spawnPoints;
        private int nbItemToSpawn;

        protected override void Awake()
        {
            base.Awake();
            Transform spc = transform.Find("SpawnPointContainer");
            if (spc == null)
                throw new Exception("ContainerItemSpawner need Empty named \"SpawnPointContainer\"");
            if (spc.childCount == 0)
                throw new Exception("ContainerItemSpawner.SpawnPointContainer at least one child");
            spawnPoints = new List<Vector3>();
            for (int i = 0; i < spc.childCount; i++)
            {
                spawnPoints.Add(spc.GetChild(i).position);
            }

            nbItemToSpawn = (int) Math.Ceiling(spawnPoints.Count * fillPercentage);
        }

        public override int GetNbItemToSpawn()
        {
            return nbItemToSpawn;
        }

        public override Vector3 SpawnCoordinate()
        {
            Vector3 pos = Vector3.zero;
            if (spawnPoints.Count > 0)
            {
                int idx = Random.Range(0, spawnPoints.Count - 1);
                pos = spawnPoints[idx];
                spawnPoints.RemoveAt(idx);
            }

            return pos;
        }
    }
}