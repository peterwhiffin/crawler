using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Enemy Settings", fileName = "NewEnemySettings")]
public class EnemySettings : ScriptableObject
{
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float IdleTime { get; private set; }
    [field: SerializeField] public float PursueSpeed { get; private set; }
    [field: SerializeField] public float FieldOfView {  get; private set; }
    [field: SerializeField] public float SightDistance { get; private set; }
    [field: SerializeField] public LayerMask EnvironmentMask { get; private set; }
    [field: SerializeField] public LayerMask PlayerMask { get; private set; }
    [field: SerializeField] public float m_AttackDistance { get; private set; }
}

