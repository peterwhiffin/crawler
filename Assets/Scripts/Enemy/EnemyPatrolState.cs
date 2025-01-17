using UnityEngine;

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        m_Enemy.Motor.SetNextPatrolTarget();
        m_Enemy.Animation.UpdateMove(true);
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

        m_Enemy.Motor.Patrol();

        if (!m_Enemy.Settings.IsPassive && m_Enemy.Motor.CanSeePlayer())
        {
            if (m_Enemy.Motor.PlayerWithinAttackRange())
            {
                m_StateMachine.ChangeState(m_Enemy.AttackState);
            }
            else
            {
                m_StateMachine.ChangeState(m_Enemy.IdleState);
            }
        }
        else if(m_Enemy.Motor.IsAtPatrolGoal())
        {
            m_StateMachine.ChangeState(m_Enemy.IdleState);
        }
    }
}
