using UnityEngine;


namespace PS1.Scripts.Network
{
    /// <summary>
    /// Handles networked behavior and synchronization for networked objects.
    /// Used by PlayerController as parent class
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public abstract class NetworkBehaviour : MonoBehaviour
    {
        protected NetworkObject NetworkObject { get; private set; }
        protected bool IsOwner => NetworkObject != null && NetworkObject.IsOwner;

        protected virtual void Awake()
        {
            NetworkObject = GetComponent<NetworkObject>();
        }

        public abstract void OnNetworkSpawn();
        public abstract void OnNetworkDespawn();
    }
}
