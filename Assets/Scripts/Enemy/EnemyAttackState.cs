using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
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

        m_Enemy.CurrentEquipment.FirePosition.up = m_Enemy.Player.transform.position - m_Enemy.transform.position;
        m_Enemy.CurrentEquipment.StartPrimaryAttack();
        if (!m_Enemy.Motor.CanSeePlayer() || !m_Enemy.Motor.PlayerWithinAttackRange())
        {
            m_StateMachine.ChangeState(m_Enemy.IdleState);
        }
    }
}
