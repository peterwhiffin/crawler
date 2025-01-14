
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Weapon Settings/Melee Settings", fileName = "NewWeaponSettings")]
public class MeleeSettings : WeaponSettings
{
    [field: SerializeField] public float AttackDistance { get; private set; }
    [field: SerializeField] public GameObject HitEffectPrefab { get; private set; }
}
