using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Weapon Settings/Gun Settings", fileName = "NewWeaponSettings")]
public class GunSettings : WeaponSettings
{
    [field: SerializeField] public Projectile ProjectilePrefab { get; private set; }
    [field: SerializeField] public float ProjectileSpeed { get; private set; }
    [field: SerializeField] public float Accuracy { get; private set; }
}
