using System.Collections.Generic;
using UnityEngine;

namespace PS2.Scripts.Weapons
{
    /// <summary>
    /// This singleton class manages the networked state of weapon controllers across multiple players in a multiplayer environment.
    /// It maintains a registry of active PlayerWeaponController instances, allowing for simulated broadcasting of weapon switch events to remote clients.
    /// </summary>
    public class WeaponNetworkManager : MonoBehaviour
    {
        public static WeaponNetworkManager Instance { get; private set; }

        // Mirroring Problem 1's registry database lookups for weapon tracking
        private readonly Dictionary<uint, PlayerWeaponController> activeControllers = new Dictionary<uint, PlayerWeaponController>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void RegisterController(uint networkId, PlayerWeaponController controller)
        {
            if (!activeControllers.ContainsKey(networkId))
            {
                activeControllers.Add(networkId, controller);
            }
        }

        public void UnregisterController(uint networkId)
        {
            activeControllers.Remove(networkId);
        }

        // Simulates broadcasting a weapon switch payload out to remote instances
        public void BroadcastWeaponSwitch(uint senderId, int slotIndex)
        {
            foreach (var entry in activeControllers)
            {
                // Send updates exclusively to matching remote representations
                if (entry.Key != senderId)
                {
                    entry.Value.ReceiveSimulatedWeaponSwitch(slotIndex);
                }
            }
        }
    }
}