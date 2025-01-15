using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerStretchState : PlayerGroundedState
{
    public PlayerStretchState(StateMachine stateMachine, Player player, InputHandler inputHandler) : base(stateMachine, player, inputHandler)
    {
    }

    private bool m_IsLaunchQueued = false;
    private bool m_HasLaunched = false;

    public override void Enter()
    {
        base.Enter();
        m_IsLaunchQueued = false;
        m_HasLaunched = false;
    }

    public override void Exit()
    {
        base.Exit();
        
    }

    public override void Update()
    {
        if (!m_IsLaunchQueued)
        {
            base.Update();
        }

        m_Player.Motor.SetRestPosition();

        if (m_InputHandler.MoveInput != Vector2.zero)
        {
            m_Player.Motor.CheckCurrentLeg();

        }

        if (m_Player.Hotbar.IsAttacking || m_InputHandler.MoveInput != Vector2.zero)
        {
            m_Player.LookAtCursor();
            m_Player.Animation.SetCapsulePosition(m_Player.PlayerInput.MoveInput);
        }
        else
        {
            m_Player.LookAtStretchDirection();

        }

        if (!m_InputHandler.StretchInput)
        {
            if (m_Player.Motor.IsPlayerBeyondLaunchThreshold())
            {
                m_IsLaunchQueued = true;
            }
            else if (!m_IsLaunchQueued)
            {
                if (m_Player.Motor.m_ReelingObject)
                {
                    m_StateMachine.ChangeState(m_Player.GrappleState);
                }
                else
                {
                    m_StateMachine.ChangeState(m_Player.IdleState);
                }
            }
        }

        if (m_HasLaunched)
        {
            m_Player.Motor.SetLaunchTimer();
            m_StateMachine.ChangeState(m_Player.FallingState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Vector2 finalForce = Vector2.zero;
        ForceMode2D forceMode = ForceMode2D.Impulse;

        if (m_IsLaunchQueued && !m_HasLaunched)
        {
            finalForce += m_Player.Motor.GetLaunchForce();
            m_Player.Motor.LaunchPlayer();
            m_HasLaunched = true;
        }
        else
        {
            if (m_InputHandler.MoveInput != Vector2.zero)
            {
                if (m_Player.Motor.IsPlayerAtMaxDistance())
                {
                   finalForce += m_Player.Motor.GetRestPositionRestraintForce();
                }

                finalForce += m_Player.Motor.GetMoveForce(m_InputHandler.MoveInput);
            }
            else
            {
                if (!m_Player.Motor.IsMouseInDeadZone())
                {
                    finalForce += m_Player.Motor.GetStretchMoveForce();
                }

                if (m_Player.Motor.IsPlayerAtMaxStretchDistance())
                {
                    finalForce += m_Player.Motor.GetRestPositionRestraintForce();
                }
            }

            forceMode = ForceMode2D.Force;
        }

        m_Player.Motor.MovePlayer(finalForce, forceMode);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }
}
