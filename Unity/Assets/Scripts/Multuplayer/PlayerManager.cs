using System.IO;
using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (photonView.IsMine) // PV.IsMine == true if controller own by local client
            CreateController();
    }

    private void CreateController()
    {
        Debug.Log("CreateController");
        PhotonNetwork.Instantiate(Path.Combine(MultiplayerManager.PhotonPrefabPath, "PlayerController"), Vector3.one,
            Quaternion.identity);
    }
}