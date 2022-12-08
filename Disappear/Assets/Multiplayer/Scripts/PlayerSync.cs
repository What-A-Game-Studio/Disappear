using System;
using Photon.Pun;
using UnityEngine;
using WAG.Core.Menus;
using WAG.Health;

namespace WAG.Multiplayer
{
    public class PlayerSync : MonoBehaviour
    {
        public bool RPCCrouch { get; set; }
        public bool RPCGrounded { get; private set; }
        public bool RPCInventoryStatus { get; private set; }
        public Vector3 RPCVelocity { get; private set; }

        public bool IsSeeker()
        {
            return (string) pv.Owner.CustomProperties["team"] == "S";
        }

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
            if (pv == null)
            {
                Debug.LogError("Need PhotonView", this);
                Debug.Break();
            }
        }

        private PhotonView pv;
        private HealthStatusController healthController;
        private Action interactAction;

        public bool IsMine => pv.IsMine;

        public void SyncVelocity(Vector3 currentVelocity)
        {
            pv.RPC(nameof(RPC_Velocity), RpcTarget.All, currentVelocity);
        }

        public void SyncCrouch(bool crouched)
        {
            pv.RPC(nameof(RPC_Crouch), RpcTarget.All, crouched);
        }
        public void SyncGround(bool grounded)
        {
            pv.RPC(nameof(RPC_Ground), RpcTarget.All, grounded);
        }

        public void SyncInventoryStatus(bool inventoryStatus)
        {
            pv.RPC(nameof(RPC_InventoryStatus), RpcTarget.All, inventoryStatus);
        }

        public void HandleInteract()
        {
            pv.RPC(nameof(RPC_Interact), RpcTarget.All);
        }

        public void SyncHealth(HeathStatus startHeathStatus)
        {
            pv.RPC(nameof(RPC_SyncHealth), RpcTarget.All, (int) startHeathStatus);
        }

        public void Defeat()
        {
            pv.RPC(nameof(RPC_Defeat), RpcTarget.All);
        }

        [PunRPC]
        private void RPC_Crouch(bool crouch)
        {
            RPCCrouch = crouch;
        }

        [PunRPC]
        private void RPC_SyncHealth(int status)
        {
            healthController.SetHealth(status, false);
            if (!pv.IsMine)
                return;
            healthController.Invoke();
        }

        [PunRPC]
        private void RPC_Teleport()
        {
            if (!pv.IsMine)
                return;
            // transform.position = PlayerSpawnerManager.Instance.ChooseRandomSpawnPosition();
        }

        [PunRPC]
        private void RPC_Defeat()
        {
            if (!pv.IsMine)
                return;
            MenuManager.Instance.OpenMenu(MenuType.Pause);
        }

        [PunRPC]
        private void RPC_Velocity(Vector3 vel)
        {
            RPCVelocity = vel;
        }

        [PunRPC]
        private void RPC_Ground(bool ground)
        {
            RPCGrounded = ground;
        }

        [PunRPC]
        private void RPC_InventoryStatus(bool inventoryStatus)
        {
            RPCInventoryStatus = inventoryStatus;
        }

        [PunRPC]
        private void RPC_Interact()
        {
            interactAction.Invoke();
        }

        public void Init(HealthStatusController hc, Action ia)
        {
            healthController = hc;
            interactAction = ia;
        }
    }
}