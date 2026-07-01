using UnityEngine;

namespace PS2.Scripts.Weapons
{
    /// <summary>
    /// Defines the configuration for a weapon, including its identity, ammo capacity, and shooting characteristics.
    /// This ScriptableObject can be used to create different weapon types with varying properties.
    /// </summary>
    public enum WeaponSlot
    {
        PrimaryOne = 0,
        PrimaryTwo = 1,
        Secondary = 2
    }

    /// <summary>
    /// Represents the configuration data for a weapon, including its name, allowed slot, ammo capacity, fire rate, range, and damage.
    /// This ScriptableObject can be used to create different weapon types with varying properties.
    /// </summary>
    [CreateAssetMenu(fileName = "NewWeaponConfig", menuName = "FPS/Weapon Config")]
    public class WeaponConfig : ScriptableObject
    {
        [Header("Identity")]
        public string weaponName;
        public WeaponSlot allowedSlot;

        [Header("Visuals")]
        public GameObject gunPrefab;

        [Header("Ammo Configurations")]
        public int magazineSize = 30;
        public int maxReserveAmmo = 90;

        [Header("Shooting Configurations")]
        public float fireRate = 0.1f;
        public float weaponRange = 100f;
        public int damage = 25;
    }
}