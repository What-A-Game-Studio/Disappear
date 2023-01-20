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
        
        private void LateUpdate()
        {
            if (IsServer)
            {
                LateUpdateServer();
            }
            
            if (IsClient)
            {
                LateUpdateClient();
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
        
        protected virtual void LateUpdateServer()
        {
        }

        protected virtual void LateUpdateClient()
        {
        }


        protected virtual void OnClientSpawn(){}
        protected virtual void OnServerSpawn(){}
        protected virtual void UpdateServer(){}
        protected virtual void UpdateClient(){}
        protected virtual void FixedUpdateServer(){}
        protected virtual void FixedUpdateClient(){}
    }
}