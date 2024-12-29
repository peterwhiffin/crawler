using UnityEngine;

public class Enemy : MonoBehaviour
{
    private StateMachine m_StateMachine;

    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyPursueState PursueState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }

    [field: SerializeField] public Equipment CurrentEquipment { get; private set; }
    [field: SerializeField] public EnemySettings Settings { get; private set; }
    [field: SerializeField] public EnemyMotor Motor { get; private set; }
    [field: SerializeField] public Player Player { get; private set; }

    private void Start()
    {
        CreateStates();
    }

    private void CreateStates()
    {
        m_StateMachine = new StateMachine();
        IdleState = new(m_StateMachine, this);
        PatrolState = new(m_StateMachine, this);
        PursueState = new(m_StateMachine, this);
        AttackState = new(m_StateMachine, this);

        m_StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        m_StateMachine.CurrentState.Update();
    }

    private void FixedUpdate()
    {
        m_StateMachine.CurrentState.FixedUpdate();
    }

    private void LateUpdate()
    {
        m_StateMachine.CurrentState.LateUpdate();
    }
}
