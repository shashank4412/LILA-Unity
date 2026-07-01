using System;
using PS1.Scripts.Network;
using UnityEngine;

namespace PS1.Scripts.Movement
{
    /// <summary>
    /// Controls player movement components, handling initialization and synchronization between local and remote movement scripts.
    /// </summary>
    public class PlayerController : NetworkBehaviour
    {
        private event Action OnInitializationTrigger;
        private event Action OnDeinitializationTrigger;

        [SerializeField] private LocalPlayer localPlayer;
        [SerializeField] private RemotePlayer remotePlayer;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                OnInitializationTrigger += localPlayer.Init;
                OnDeinitializationTrigger += localPlayer.DeInit;

                localPlayer.OnPositionBroadcast += SendPositionPayloadToServer;
                if (remotePlayer != null) remotePlayer.enabled = false;
            }
            else
            {
                OnInitializationTrigger += remotePlayer.Init;
                OnDeinitializationTrigger += remotePlayer.DeInit;

                if (localPlayer != null) localPlayer.enabled = false;
            }

            OnInitializationTrigger?.Invoke();
        }

        private void SendPositionPayloadToServer(PositionPayload payload)
        {
            if (NetworkManager.Instance != null && NetworkObject != null)
            {
                NetworkManager.Instance.BroadcastPositionPayload(NetworkObject.NetworkObjectId, payload);
            }
        }

        public void ReceiveSimulatedPositionPayload(PositionPayload payload)
        {
            if (!IsOwner && remotePlayer != null)
            {
                remotePlayer.SetNetworkPosition(payload);
            }
        }

        public override void OnNetworkDespawn()
        {
            OnDeinitializationTrigger?.Invoke();

            if (IsOwner)
            {
                OnInitializationTrigger -= localPlayer.Init;
                OnDeinitializationTrigger -= localPlayer.DeInit;

                localPlayer.OnPositionBroadcast -= SendPositionPayloadToServer;
            }
            else
            {
                OnInitializationTrigger -= remotePlayer.Init;
                OnDeinitializationTrigger -= remotePlayer.DeInit;
            }
        }
    }
}
