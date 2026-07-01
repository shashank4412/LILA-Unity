using System;
using PS1.Scripts.Network; // Accessing your Problem 1 network context
using PS2.Scripts.UI;
using UnityEngine;

namespace PS2.Scripts.Weapons
{
    /// <summary>
    /// Controls the player's weapon inventory, allowing for weapon switching, firing, and reloading.
    /// Integrates with the network system to synchronize weapon states across clients.
    /// </summary>
    public class PlayerWeaponController : NetworkBehaviour
    {
        [Header("Inventory Setup")]
        [SerializeField] private WeaponConfig[] startingWeapons = new WeaponConfig[3];
        [SerializeField] private Transform weaponGripPoint;
        // [SerializeField] private GameObject weaponInstancePrefab;

        private WeaponInstance[] equippedWeapons = new WeaponInstance[3];
        private int currentSlotIndex = -1;

        public event Action<WeaponInstance> OnActiveWeaponSwitched;
        public event Action<int, string> OnSlotAssigned;

        public override void OnNetworkSpawn()
        {
            // 1. Register this component context safely into the weapon network system first
            if (WeaponNetworkManager.Instance != null && NetworkObject != null)
            {
                WeaponNetworkManager.Instance.RegisterController(NetworkObject.NetworkObjectId, this);
            }

            // 2. CONNECT THE HUD FIRST: So it is actively listening before weapons are created
            if (IsOwner)
            {
                WeaponHUD hud = FindObjectOfType<WeaponHUD>();
                if (hud != null) hud.AssignTargetPlayer(this);
            }

            // 3. INITIALIZE DEFAULT LOADOUT LAST: Now the HUD will catch every single OnSlotAssigned event!
            InitializeDefaultLoadout();
        }

        public override void OnNetworkDespawn()
        {
            // Unregister from lookups on despawn
            if (WeaponNetworkManager.Instance != null && NetworkObject != null)
                WeaponNetworkManager.Instance.UnregisterController(NetworkObject.NetworkObjectId);
        }

        private void InitializeDefaultLoadout()
        {
            for (int i = 0; i < startingWeapons.Length; i++)
            {
                if (startingWeapons[i] == null) continue;

                // SAFETY CHECK: Make sure the designer actually linked a prefab to the ScriptableObject
                if (startingWeapons[i].gunPrefab == null)
                {
                    Debug.LogError($"[WEAPON ERROR] {startingWeapons[i].weaponName} is missing its gunPrefab reference!");
                    continue;
                }

                // SPAWN THE SPECIFIC PREFAB: Instantiates the unique 3D gun model directly
                GameObject wpnObj = Instantiate(startingWeapons[i].gunPrefab, weaponGripPoint);
                WeaponInstance instance = wpnObj.GetComponent<WeaponInstance>();
                if (instance != null)
                {
                    instance.Initialize(startingWeapons[i]);
                    equippedWeapons[i] = instance;
                }
                wpnObj.SetActive(false);

                Debug.Log($"Slot {i} assigned to weapon: {startingWeapons[i].weaponName}");
                OnSlotAssigned?.Invoke(i, startingWeapons[i].weaponName);
            }

            // Explicit initial assignment bypasses the broadcast routine
            ExecuteWeaponSwap(0);
        }

        public void HandleFiringInput()
        {
            if (currentSlotIndex == -1 || equippedWeapons[currentSlotIndex] == null) return;
            equippedWeapons[currentSlotIndex].Fire();
        }

        public void SwitchWeapon(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= equippedWeapons.Length) return;
            if (equippedWeapons[slotIndex] == null || slotIndex == currentSlotIndex) return;

            // 1. Instantly swap visuals locally
            ExecuteWeaponSwap(slotIndex);

            // 2. Broadcast data down the network line if this is the active owner instance
            if (IsOwner && NetworkObject != null && WeaponNetworkManager.Instance != null)
                WeaponNetworkManager.Instance.BroadcastWeaponSwitch(NetworkObject.NetworkObjectId, slotIndex);
        }

        // Handles the core structural visual modification
        private void ExecuteWeaponSwap(int slotIndex)
        {
            if (currentSlotIndex != -1 && equippedWeapons[currentSlotIndex] != null)
                equippedWeapons[currentSlotIndex].gameObject.SetActive(false);

            currentSlotIndex = slotIndex;
            equippedWeapons[currentSlotIndex].gameObject.SetActive(true);

            OnActiveWeaponSwitched?.Invoke(equippedWeapons[currentSlotIndex]);
        }

        // Inbound execution layer called on remote client objects by WeaponNetworkManager
        public void ReceiveSimulatedWeaponSwitch(int slotIndex)
        {
            if (!IsOwner) ExecuteWeaponSwap(slotIndex);
        }

        public void HandleReloadInput()
        {
            if (currentSlotIndex == -1 || equippedWeapons[currentSlotIndex] == null) return;
            equippedWeapons[currentSlotIndex].Reload();
        }
    }
}