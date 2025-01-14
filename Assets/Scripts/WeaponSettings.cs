using UnityEngine;

public class WeaponSettings : ScriptableObject
{
    [field: SerializeField] public float FireRate { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }   
    [field: SerializeField] public float HitCheckRadius { get; private set; }      
    [field: SerializeField] public AudioClip FireSound { get; private set; }
    [field: SerializeField] public LayerMask HitMask { get; private set; }
    [field: SerializeField] public float MinimumAudioTime { get; private set; }
    [field: SerializeField] public bool IsAudioOneShot { get; private set; }    
}
