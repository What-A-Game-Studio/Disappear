using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace WAG.Multiplayer
{
    public abstract class NetworkSideBehaviour : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                OnServerSpawn();
            }
            else
            {
                OnClientSpawn();
            }
        }

        void Update()
        {
            if (IsServer)
            {
                UpdateServer();
            }
            else
            {
                UpdateClient();
            }
        }

        private void FixedUpdate()
        {
            if (IsServer || IsHost)
            {
                FixedUpdateServer();
            }
            else
            {
                FixedUpdateClient();
            }
        }

        protected abstract void OnClientSpawn();
        protected abstract void OnServerSpawn();
        protected abstract void UpdateServer();
        protected abstract void UpdateClient();
        protected abstract void FixedUpdateServer();
        protected abstract void FixedUpdateClient();
    }
}