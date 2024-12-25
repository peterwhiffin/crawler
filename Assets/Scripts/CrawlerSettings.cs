using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Crawler Settings", fileName = "CrawlerSettings")]
public class CrawlerSettings : ScriptableObject
{
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float MinimumLegMoveDistance { get; private set; }
    [field: SerializeField] public float LegMoveRate { get; private set; }
    [field: SerializeField] public float LegMoveThreshold { get; private set; }
    [field: SerializeField] public float LegReach { get; private set; }
    [field: SerializeField] public float SpringForce { get; private set; }
    [field: SerializeField] public float DampForce { get; private set; }
    [field: SerializeField] public float MaxSpringStretch { get; private set; }
    [field: SerializeField] public float WallSuspensionDistance { get; private set; }
    [field: SerializeField] public float WallSuspensionSpringStrength { get; private set; }
    [field: SerializeField] public float WallSuspensionDamperStrength { get; private set; }
}
