using System.Collections.Generic;
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
    [field: SerializeField] public float LaunchForce { get; private set; }
    [field: SerializeField] public float LaunchThreshold { get; private set; }
    [field: SerializeField] public float LegLaunchSpringStrength { get; private set; }
    [field: SerializeField] public float LegLaunchDamperStrength { get; private set; }
    [field: SerializeField] public float LaunchHitVelocityThreshold { get; private set; }
    [field: SerializeField] public float CrosshairLookThreshold { get; private set; }
    [field: SerializeField] public float MaxCollisionAngle { get; private set; }
    [field: SerializeField] public float LegSearchMultiplier { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public List<AudioClip> FootStepClips { get; private set; }
    [field: SerializeField] public float FootStepRate { get; private set; }
    [field: SerializeField] public float CameraSmoothRate { get; private set; }
    [field: SerializeField] public float MaxCameraDistance { get; private set; }   
    [field: SerializeField] public Vector2 CrosshairCameraRange { get; private set; }
    [field: SerializeField] public float MaxCameraSpeed { get; private set; }
    [field: SerializeField] public float MaxGraphicLocalStretch {  get; private set; }
    [field: SerializeField] public float LocalStretchLerp { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public float LegLaunchRestDistance { get; private set; }
    [field: SerializeField] public float LegLinearDamping { get; private set; }
    [field: SerializeField] public float InAirMoveSpeed { get; private set; }
    [field: SerializeField] public float MaxStretchDistance { get; private set; }
    [field: SerializeField] public float GrappleForce { get; private set; }
    [field: SerializeField] public float LaunchTimeout { get; private set; }
    [field: SerializeField] public AnimationCurve LegMoveRateCurve { get; private set; }
    [field: SerializeField] public float GrappleStretchDistance { get; private set; }
    [field: SerializeField] public float GrappleInAirMoveSpeed { get; private set; }
    [field: SerializeField] public float GrappleSpringStrength { get; private set; }
    [field: SerializeField] public float GrappleDamperStrength { get; private set; }
    [field: SerializeField] public float InAirLinearDamping { get; private set; }
    [field: SerializeField] public float OnGroundLinearDamping { get; private set; }
    [field: SerializeField] public float MaxInAirSpeed { get; private set; }
    [field: SerializeField] public float JumpInterval {  get; private set; }
}
