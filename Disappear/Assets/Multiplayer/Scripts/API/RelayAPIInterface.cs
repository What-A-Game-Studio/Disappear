using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;


namespace WAG.Multiplayer
{
    public static class RelayAPIInterface
    {
        public static async Task<string> CreateRelay(int maxConnections, string region = null)
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartHost();
                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
                return null;
            }
        }

        public static async void JoinRelay(string relayCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);
                RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}