using UnityEngine;
using Photon.Pun;
using System.IO;

namespace WAG.Multiplayer
{
    [RequireComponent(typeof(PhotonView))]
    public class PlayerManager : MonoBehaviour
    {
        private PhotonView pv;

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (pv.IsMine) // PV.IsMine == true if controller own by local client
            {
                CreateController();
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        void CreateController()
        {
            Debug.Log("CreateController");
            GameObject player = PhotonNetwork.Instantiate(
                Path.Combine(MultiplayerManager.PhotonPrefabPath,
                    "PlayerController"),
                PlayerSpawnerManager.Instance.ChooseRandomSpawnPosition(), Quaternion.identity);
        }
    }
}