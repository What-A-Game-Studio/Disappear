using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using WAG.Core.GM;
using WAG.Menu;
using WAG.Health;

namespace WAG.Multiplayer
{
    public class NGOPlayerSync : NetworkBehaviour
    {
        [field: SerializeField]
        public NetworkVariable<Vector2> RPCMove { get; private set; } = new NetworkVariable<Vector2>();

        [field: SerializeField]
        public NetworkVariable<Vector3> RPCVelocity { get; private set; } = new NetworkVariable<Vector3>();

        [field: SerializeField]
        public NetworkVariable<bool> RPCJump { get; private set; } = new NetworkVariable<bool>();

        [field: SerializeField]
        public NetworkVariable<bool> RPCCrouch { get; private set; } = new NetworkVariable<bool>();

        [field: SerializeField]
        public NetworkVariable<bool> RPCInventoryStatus { get; private set; } = new NetworkVariable<bool>();

        [field: SerializeField]
        public NetworkVariable<float> RPCRotation { get; private set; } = new NetworkVariable<float>();

        private NGOHealthStatusController healthController;
        private Action interactAction;

        public bool IsSeeker()
        {
            return true;
            // return ["Role"] == "S";
        }

        public void SyncMove(Vector2 move)
        {
            MoveServerRpc(move);
        }

        public void SyncRotation(float rotation)
        {
            RotateServerRpc(rotation);
        }


        public void SyncVelocity(Vector3 currentVelocity)
        {
            VelocityServerRpc(currentVelocity);
        }

        public void SyncJump(bool jump)
        {
            JumpServerRpc(jump);
        }


        public void SyncCrouch(bool crouched)
        {
            CrouchServerRpc(crouched);
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
        private void RotateServerRpc(float rotation)
        {
            RPCRotation.Value = rotation;
        }

        [ServerRpc]
        private void VelocityServerRpc(Vector3 vel)
        {
            RPCVelocity.Value = vel;
        }

        [ServerRpc]
        private void JumpServerRpc(bool jump)
        {
            RPCJump.Value = jump;
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