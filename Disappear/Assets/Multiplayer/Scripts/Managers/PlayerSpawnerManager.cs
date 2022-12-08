using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WAG.Multiplayer
{
    public class PlayerSpawnerManager : MonoBehaviour
    {
        public static PlayerSpawnerManager Instance { get; protected set; }

        private List<Vector3> playerSpawnPositionList = new List<Vector3>();


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            if (transform.childCount == 0)
            {
                Debug.LogError("PlayerSpawnerManager need some spawners", this);
                return;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                playerSpawnPositionList.Add(transform.GetChild(i).position);
            }
        }

        public Vector3 ChooseRandomSpawnPosition()
        {
            int rand = Random.Range(0, playerSpawnPositionList.Count);
            Vector3 newPos = playerSpawnPositionList[rand];
            return newPos;
        }
    }
}