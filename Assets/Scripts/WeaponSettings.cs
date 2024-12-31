using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Weapon Settings", fileName = "NewWeaponSettings")]
public class WeaponSettings : ScriptableObject
{
    [field: SerializeField] public float FireRate { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float ProjectileSpeed { get; private set; }
    [field: SerializeField] public float ProjectileRadius { get; private set; }
    [field: SerializeField] public float Accuracy { get; private set; }
    [field: SerializeField] public Projectile ProjectilePrefab { get; private set; }
    [field: SerializeField] public AudioClip FireSound { get; private set; }
    [field: SerializeField] public float MinimumAudioTime { get; private set; }
}
