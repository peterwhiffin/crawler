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
    [field: SerializeField] public float MaxLegDistance { get; private set; }
    [field: SerializeField] public AnimationCurve StepCurve { get; private set; }
    [field: SerializeField] public float StepHeight { get; private set; }
    [field: SerializeField] public float RopeOffset { get; private set; }
    [field: SerializeField] public float MaxLegTime { get; private set; }
    [field: SerializeField] public float LaunchForce { get; private set; }
    [field: SerializeField] public float LaunchTime { get; private set; }
    [field: SerializeField] public float LaunchThreshold { get; private set; }
    [field: SerializeField] public float LegLaunchSpringStrength { get; private set; }
    [field: SerializeField] public float LegLaunchDamperStrength { get; private set; }
    [field: SerializeField] public float LaunchHitVelocityThreshold { get; private set; }
    [field: SerializeField] public float LegAngularDampStrength { get; private set; }
    [field: SerializeField] public float LookRotationThreshold { get; private set; }
    [field: SerializeField] public float SlingShotMouseThreshold { get; private set; }
    [field: SerializeField] public float CrosshairLookThreshold { get; private set; }
    [field: SerializeField] public float CollisionResetTime { get; private set; }
    [field: SerializeField] public float MaxCollisionAngle { get; private set; }
    [field: SerializeField] public float LegSearchMultiplier { get; private set; }
}
