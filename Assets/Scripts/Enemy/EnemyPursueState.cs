using UnityEngine;

public class EnemyPursueState : EnemyState
{
    public EnemyPursueState(StateMachine stateMachine, Enemy enemy) : base(stateMachine, enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        m_Enemy.Motor.CachePlayerPosition(m_Enemy.Player.transform.position);
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

        if (m_Enemy.Motor.PlayerWithinView())
        {
            m_Enemy.Motor.CachePlayerPosition(m_Enemy.Player.transform.position);

            if (m_Enemy.Motor.IsAtAttackDistance())
            {

            }
            else
            {
                m_Enemy.Motor.PursuePlayer();
            }
        }
        else
        {

        }
    }
}
