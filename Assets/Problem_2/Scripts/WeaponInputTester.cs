using PS1.Scripts.Network;
using UnityEngine;

namespace PS2.Scripts.Weapons
{
    /// <summary>
    /// A testing utility for weapon input handling, allowing developers to simulate weapon switching, firing, and reloading actions.
    /// This component should be attached to a player GameObject that has a PlayerWeaponController.
    /// It listens for specific input keys and invokes the corresponding methods on the PlayerWeaponController.
    /// </summary>
    [RequireComponent(typeof(PlayerWeaponController))]
    public class WeaponInputTester : NetworkBehaviour
    {
        private PlayerWeaponController weaponController;

        protected override void Awake()
        {
            base.Awake();
            weaponController = GetComponent<PlayerWeaponController>();
        }

        public override void OnNetworkSpawn() { }
        public override void OnNetworkDespawn() { }

        private void Update()
        {
            // Only process input controls if this script belongs to the local user
            if (!IsOwner) return;

            // Weapon Switching Hotkeys
            if (Input.GetKeyDown(KeyCode.Alpha1)) weaponController.SwitchWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) weaponController.SwitchWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) weaponController.SwitchWeapon(2);

            // Action Execution Hotkeys
            if (Input.GetMouseButton(0)) weaponController.HandleFiringInput(); // Left Click to fire
            if (Input.GetKeyDown(KeyCode.R)) weaponController.HandleReloadInput(); // R to reload
        }
    }
}