using PS2.Scripts.Weapons;
using UnityEngine;

namespace PS2.Scripts.UI
{
    public class WeaponHUD : MonoBehaviour
    {
        /// <summary>
        /// This class is responsible for rendering the player's weapon inventory HUD on the screen.
        /// It listens to the PlayerWeaponController for updates on the active weapon and slot assignments,
        /// and dynamically displays the current loadout and ammo counts in a user-friendly format.
        /// </summary>
        private PlayerWeaponController monitoredController;
        private WeaponInstance currentlyObservedWeapon;

        // Cache slot status strings locally for the overlay display
        private string[] slotDisplayNames = new string[3] { "Empty", "Empty", "Empty" };

        public void AssignTargetPlayer(PlayerWeaponController controller)
        {
            if (monitoredController != null) UnsubscribeFromEvents();

            monitoredController = controller;
            monitoredController.OnActiveWeaponSwitched += HandleWeaponSwitch;
            monitoredController.OnSlotAssigned += HandleSlotRegistered;
        }

        private void HandleSlotRegistered(int slotIndex, string weaponName)
        {
            if (slotIndex >= 0 && slotIndex < slotDisplayNames.Length)
            {
                slotDisplayNames[slotIndex] = weaponName;
                Debug.Log($"Slot {slotIndex} assigned to weapon: {weaponName}");
            }
        }

        private void HandleWeaponSwitch(WeaponInstance targetWeapon)
        {
            currentlyObservedWeapon = targetWeapon;
        }

        // Draws the UI interface dynamically on screen every frame
        private void OnGUI()
        {
            if (monitoredController == null) return;

            // --- 1. Draw Background Box Container ---
            GUI.Box(new Rect(20, 20, 280, 170), "WEAPON INVENTORY HUD");

            // --- 2. List Equipped Loadout Status ---
            GUI.Label(new Rect(35, 50, 250, 20), $"[1] Primary Slot 1 :  {slotDisplayNames[0]}");
            GUI.Label(new Rect(35, 70, 250, 20), $"[2] Primary Slot 2 :  {slotDisplayNames[1]}");
            GUI.Label(new Rect(35, 90, 250, 20), $"[3] Secondary Slot  :  {slotDisplayNames[2]}");

            // Decorative separating line
            GUI.Label(new Rect(35, 110, 250, 20), "---------------------------------------");

            // --- 3. Draw Active Weapon & Real-time Ammo ---
            if (currentlyObservedWeapon != null && currentlyObservedWeapon.Config != null)
            {
                GUI.Label(new Rect(35, 130, 250, 20), $"ACTIVE GUN:  <b>{currentlyObservedWeapon.Config.weaponName}</b>");

                // Emphasize ammo numbers with custom bold formatting
                GUIStyle ammoStyle = new GUIStyle(GUI.skin.label) { richText = true, fontSize = 14 };
                GUI.Label(new Rect(35, 150, 250, 25), $"AMMO COUNT: <color=yellow><b>{currentlyObservedWeapon.CurrentClipAmmo}</b></color> / {currentlyObservedWeapon.CurrentReserveAmmo}", ammoStyle);
            }
            else
            {
                GUI.Label(new Rect(35, 130, 250, 20), "ACTIVE GUN: None");
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (monitoredController != null)
            {
                monitoredController.OnActiveWeaponSwitched -= HandleWeaponSwitch;
                monitoredController.OnSlotAssigned -= HandleSlotRegistered;
            }
        }

        private void OnDestroy() => UnsubscribeFromEvents();
    }
}