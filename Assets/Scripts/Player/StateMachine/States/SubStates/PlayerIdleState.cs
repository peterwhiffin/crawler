using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(StateMachine stateMachine, Player player, InputHandler inputHandler) : base(stateMachine, player, inputHandler)
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

    public override void Update()
    {
        base.Update();


        m_Player.Animation.SetCapsulePosition(Vector2.zero);

        if (m_Player.Motor.LegNeedsToMove())
        {
            m_Player.Motor.CheckCurrentLeg();
        }
        else if (m_Player.Motor.HasPlayerHitMoveThreshold())
        {
            //m_Player.Motor.CheckCurrentLeg();
        }

        m_Player.Motor.CheckLegsIdle();
        m_Player.Motor.SetRestPosition();
        m_Player.LookAtCursor();

        if (m_Player.Motor.m_ReelingObject)
        {
            m_StateMachine.ChangeState(m_Player.GrappleState);
        }
        else if (m_InputHandler.StretchInput)
        {
            m_StateMachine.ChangeState(m_Player.StretchState);
        }
        else if (m_InputHandler.JumpInput && m_Player.Motor.CanJump())
        {
            m_StateMachine.ChangeState(m_Player.JumpState);
        }
        else if(m_InputHandler.MoveInput != Vector2.zero)
        {
            m_StateMachine.ChangeState(m_Player.CrawlState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 finalForce = m_Player.Motor.GetPlayerFloatForce();

        if (m_Player.Motor.IsPlayerAtMaxDistance())
        {
            finalForce += m_Player.Motor.GetRestPositionRestraintForce();
        }

        if (m_Player.Motor.IsGrappled)
        {
            finalForce += m_Player.Motor.GetGrappleConstraint();
        }

        m_Player.Motor.MovePlayer(finalForce);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    } 
}
