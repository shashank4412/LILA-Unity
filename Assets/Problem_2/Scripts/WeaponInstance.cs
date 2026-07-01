using System;
using UnityEngine;

namespace PS2.Scripts.Weapons
{
    /// <summary>
    /// Represents an instance of a weapon in the game, managing its state and behavior.
    /// </summary>
    public class WeaponInstance : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform firePoint;

        public WeaponConfig Config { get; private set; }

        // Dynamic States
        public int CurrentClipAmmo { get; private set; }
        public int CurrentReserveAmmo { get; private set; }
        private float nextFireTime;

        // Events for UI and FX decoupling
        public event Action OnWeaponFired;
        public event Action OnAmmoChanged;

        public void Initialize(WeaponConfig config)
        {
            Config = config;
            CurrentClipAmmo = config.magazineSize;
            CurrentReserveAmmo = config.maxReserveAmmo;
            OnAmmoChanged?.Invoke();
        }

        public bool CanFire()
        {
            return CurrentClipAmmo > 0 && Time.time >= nextFireTime;
        }

        public void Fire()
        {
            if (!CanFire()) return;

            CurrentClipAmmo--;
            nextFireTime = Time.time + Config.fireRate;

            OnWeaponFired?.Invoke();
            OnAmmoChanged?.Invoke();

            // Perform raycast/projectile logic here
            ExecuteRaycastShoot();
        }

        public void Reload()
        {
            if (CurrentClipAmmo == Config.magazineSize || CurrentReserveAmmo <= 0) return;

            int ammoNeeded = Config.magazineSize - CurrentClipAmmo;
            int ammoToTransfer = Mathf.Min(ammoNeeded, CurrentReserveAmmo);

            CurrentClipAmmo += ammoToTransfer;
            CurrentReserveAmmo -= ammoToTransfer;

            OnAmmoChanged?.Invoke();
        }

        private void ExecuteRaycastShoot()
        {
            // Raycast processing code goes here
        }
    }
}