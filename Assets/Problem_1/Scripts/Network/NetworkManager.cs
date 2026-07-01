using System.Collections.Generic;
using PS1.Scripts.Movement;
using UnityEngine;

namespace PS1.Scripts.Network
{
    /// <summary>
    /// Manages networked player entities and handles synchronization of player transforms across the network.
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        [SerializeField] private GameObject playerPrefab;

        // Registry tracking all active network entities currently online
        private readonly Dictionary<uint, NetworkObject> activeNetworkObjects = new Dictionary<uint, NetworkObject>();
        private uint nextNetworkId = 0;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            SpawnPlayer(assignAsOwner: true);  // Spawn Local Owner
            SpawnPlayer(assignAsOwner: false); // Spawn Remote Player
        }

        private void SpawnPlayer(bool assignAsOwner, Vector3? spawnPos = null)
        {
            // Spawn a new player object with self identity
            spawnPos = (spawnPos == null) ? playerPrefab.transform.position : spawnPos.Value;

            // Instantiate player prefab and setup network identity
            GameObject spawnedClone = Instantiate(playerPrefab, spawnPos.Value, Quaternion.identity);
            NetworkObject netObj = spawnedClone.GetComponent<NetworkObject>();
            uint assignedId = nextNetworkId++;
            netObj.SetupNetworkIdentity(assignedId, assignAsOwner);

            // Register inside the network database lookups
            activeNetworkObjects.Add(assignedId, netObj);

            // Pass 'true' into the parameter to include disabled/inactive components
            NetworkBehaviour[] netBehaviours = spawnedClone.GetComponentsInChildren<NetworkBehaviour>(true);
            foreach (var behaviour in netBehaviours)
                behaviour.OnNetworkSpawn();
        }

        // Simulates broadcasting a transform data payload out to the server/other instances
        public void BroadcastPositionPayload(uint senderId, PositionPayload payload)
        {
            foreach (KeyValuePair<uint, NetworkObject> entry in activeNetworkObjects)
            {
                // Send tracking updates exclusively to the matching remote object representations
                if (entry.Key != senderId)
                {
                    // Locating target component context safely
                    var remoteController = entry.Value.GetComponent<PlayerController>();
                    if (remoteController != null)
                        remoteController.ReceiveSimulatedPositionPayload(payload);
                }
            }
        }
    }
}