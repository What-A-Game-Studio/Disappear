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

            // else
            {
                OnClientSpawn();
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
                OnServerDespawn();
            else
                OnClientDespawn();
            OnDespawn();
        }

        protected virtual void OnClientDespawn()
        {
        }

        protected virtual void OnDespawn()
        {
        }
        protected virtual void OnServerDespawn()
        {
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


        protected virtual void OnClientSpawn(){}
        protected virtual void OnServerSpawn(){}
        protected virtual void UpdateServer(){}
        protected virtual void UpdateClient(){}
        protected virtual void FixedUpdateServer(){}
        protected virtual void FixedUpdateClient(){}
    }
}