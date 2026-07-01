using UnityEngine;

namespace PS1.Scripts.Network
{
    /// <summary>
    /// Represents a networked object with unique identification and ownership status.
    /// It will be automatically added to the GameObject because of NetworkBehaviour,
    /// NetworkBehaviour is used by PlayerController
    /// </summary>
    public class NetworkObject : MonoBehaviour
    {
        [HideInInspector] public uint NetworkObjectId { get; private set; }
        [HideInInspector] public bool IsOwner { get; private set; }

        public void SetupNetworkIdentity(uint id, bool identityOwnerStatus)
        {
            NetworkObjectId = id;
            IsOwner = identityOwnerStatus;
        }
    }
}