using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using WAG.Menu;
using WAG.Health;

namespace WAG.Multiplayer
{
    public class NGOPlayerSync : NetworkBehaviour
    {
        public NetworkVariable<FixedString32Bytes> RPCName { get; set; }
        public NetworkVariable<Vector2> RPCMove { get; set; }
        public NetworkVariable<Vector3> RPCVelocity { get; private set; }
        public NetworkVariable<bool> RPCGrounded { get; private set; }
        public NetworkVariable<bool> RPCCrouch { get; set; }
        public NetworkVariable<bool> RPCInventoryStatus { get; private set; }


        private NGOHealthStatusController healthController;
        private Action interactAction;

        public bool IsMine => IsOwner;

        public bool IsSeeker()
        {
            return true;
            // return ["Role"] == "S";
        }

        public void SyncName(string name)
        {
            NamePlayerServerRpc(name);
        }


        [ServerRpc]
        private void NamePlayerServerRpc(string name)
        {
            RPCName.Value = name;
        }

        public void SyncMove(Vector2 move)
        {
            MoveServerRpc(move);
        }

        public void SyncVelocity(Vector3 currentVelocity)
        {
            VelocityServerRpc(currentVelocity);
        }

        public void SyncCrouch(bool crouched)
        {
            CrouchServerRpc(crouched);
        }

        public void SyncGround(bool grounded)
        {
            GroundServerRpc(grounded);
        }

        public void SyncInventoryStatus(bool inventoryStatus)
        {
            InventoryStatusServerRpc(inventoryStatus);
        }

        public void HandleInteract()
        {
            InteractServerRpc();
        }

        public void SyncHealth(HeathStatus startHeathStatus)
        {
            SyncHealthServerRpc((int)startHeathStatus);
        }

        public void Defeat()
        {
            DefeatServerRpc();
        }

        [ServerRpc]
        private void CrouchServerRpc(bool crouch)
        {
            RPCCrouch.Value = crouch;
        }

        [ServerRpc]
        private void SyncHealthServerRpc(int status)
        {
            healthController.SetHealth(status, false);
            if (!IsOwner)
                return;
            healthController.Invoke();
        }

        [ServerRpc]
        private void TeleportServerRpc()
        {
            if (!IsOwner)
                return;
            // transform.position = PlayerSpawnerManager.Instance.ChooseRandomSpawnPosition();
        }

        [ServerRpc]
        private void DefeatServerRpc()
        {
            if (!IsOwner)
                return;
            MenuManager.Instance.OpenMenu(MenuType.Pause);
        }

        [ServerRpc]
        private void MoveServerRpc(Vector2 move)
        {
            RPCMove.Value = move;
        }

        [ServerRpc]
        private void VelocityServerRpc(Vector3 vel)
        {
            RPCVelocity.Value = vel;
        }

        [ServerRpc]
        private void GroundServerRpc(bool ground)
        {
            RPCGrounded.Value = ground;
        }

        [ServerRpc]
        private void InventoryStatusServerRpc(bool inventoryStatus)
        {
            RPCInventoryStatus.Value = inventoryStatus;
        }

        [ServerRpc]
        private void InteractServerRpc()
        {
            interactAction.Invoke();
        }

        public void Init(NGOHealthStatusController hc, Action ia)
        {
            healthController = hc;
            interactAction = ia;
        }
    }
}