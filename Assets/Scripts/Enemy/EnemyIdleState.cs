using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
    }

    private float m_EnterTime = 0f;

    public override void Enter()
    {
        base.Enter();
        m_EnterTime = Time.time;
        m_Enemy.Motor.ClearPlayerPositionHistory();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void Update()
    {
        base.Update();

        if (m_Enemy.Motor.CanSeePlayer())
        {
            if (m_Enemy.Motor.PlayerWithinAttackRange())
            {
                m_StateMachine.ChangeState(m_Enemy.AttackState);
            }
        }
        else if (Time.time - m_EnterTime > m_Enemy.Settings.IdleTime)
        {
            Debug.Log("going to patrol");
            m_StateMachine.ChangeState(m_Enemy.PatrolState);
        }
    }
}
