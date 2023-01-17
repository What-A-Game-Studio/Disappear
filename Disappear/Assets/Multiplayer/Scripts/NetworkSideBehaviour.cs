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

            if (IsClient)
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

            if (IsClient)
            {
                UpdateClient();
            }
        }

        private void FixedUpdate()
        {
            if (IsServer)
            {
                FixedUpdateServer();
            }

            if (IsClient)
            {
                FixedUpdateClient();
            }
        }

        protected virtual void OnClientSpawn()
        {
        }

        protected virtual void OnServerSpawn()
        {
        }

        protected virtual void UpdateServer()
        {
        }

        protected virtual void UpdateClient()
        {
        }

        protected virtual void FixedUpdateServer()
        {
        }

        protected virtual void FixedUpdateClient()
        {
        }
    }
}