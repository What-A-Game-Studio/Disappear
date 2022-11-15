using Photon.Pun;
using UnityEngine;
using WAG.Core.Controls;
public class CrouchController : MonoBehaviour
{
    [SerializeField] private float crouchSpeedFactor = -0.5f;
    public float CrouchSpeedFactor => Crouched ? crouchSpeedFactor : 0;

    [SerializeField] private float capsuleColliderStandRadius = .28f;
    [SerializeField] private float capsuleColliderStandHeight = 1.78f;

    [SerializeField] private float capsuleColliderCrouchRadius = .42f;
    [SerializeField] private float capsuleColliderCrouchHeight = .89f;
    private CapsuleCollider capsuleCollider;

    private bool rpcCrouch;
    public bool Crouched => pv.IsMine ? InputManager.Instance.Crouch : rpcCrouch;

    private PhotonView pv;

    private void Awake()
    {
        if (!TryGetComponent<CapsuleCollider>(out capsuleCollider))
        {
            Debug.LogError("Need CapsuleCollider", this);
            Debug.Break();
        }

        capsuleCollider.height = capsuleColliderStandRadius;
        capsuleCollider.radius = capsuleColliderStandHeight;

        if (!TryGetComponent<PhotonView>(out pv))
        {
            Debug.LogError("Need PhotonView", this);
            Debug.Break();
        }
    }


    // Update is called once per frame
    private void Update()
    {
        if (Crouched)
        {
            capsuleCollider.height = capsuleColliderCrouchHeight;
            capsuleCollider.radius = capsuleColliderCrouchRadius;
        }
        else
        {
            capsuleCollider.height = capsuleColliderStandHeight;
            capsuleCollider.radius = capsuleColliderStandRadius;
        }

        capsuleCollider.center = new Vector3(0f, capsuleCollider.height / 2, 0f);
    }

    private void FixedUpdate()
    {
        pv.RPC(nameof(RPC_Crouch), RpcTarget.All, Crouched);
    }

    [PunRPC]
    private void RPC_Crouch(bool crouch)
    {
        rpcCrouch = crouch;
    }
}