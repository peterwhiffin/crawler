using UnityEngine;

public class PlayerFallingState : PlayerInAirState
{
    public PlayerFallingState(StateMachine stateMachine, Player player, InputHandler inputHandler) : base(stateMachine, player, inputHandler)
    {
    }

    private float m_TimeEntered;

    public override void Enter()
    {
        base.Enter();
        m_TimeEntered = Time.time;
        m_Player.Motor.PlayerInAir();
    }

    public override void Exit()
    {
        base.Exit();
        m_Player.Motor.EndLaunch();
    }

    public override void Update()
    {
        base.Update();
        m_Player.Motor.SetRestPosition();
        m_Player.LookAtCursor();

        //if (Time.time - m_TimeEntered > m_Player.Motor.GetLaunchTime() && m_Player.Motor.IsPlayersVelocityBelowLandingThreshold())
        //{

        //    m_Player.Motor.CheckLegsWithoutPosition();
        //    m_Player.Motor.CheckCurrentLeg();

        //    if (m_Player.Motor.EnoughLegsToWalk())
        //    {
        //        m_StateMachine.ChangeState(m_Player.IdleState);
        //    }

        //}

        if (m_Player.Motor.CanPlayerLand(m_TimeEntered))
        {
            m_Player.Motor.CheckLegsWithoutPosition();
            m_Player.Motor.CheckCurrentLeg();

            if (m_Player.Motor.EnoughLegsToWalk())
            {
                m_StateMachine.ChangeState(m_Player.IdleState);
            }

        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }
}
