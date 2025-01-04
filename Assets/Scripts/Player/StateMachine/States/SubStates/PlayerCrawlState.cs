using UnityEngine;

public class PlayerCrawlState : PlayerGroundedState
{
    public PlayerCrawlState(StateMachine stateMachine, Player player, InputHandler inputHandler) : base(stateMachine, player, inputHandler)
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
        m_Player.Motor.CheckCurrentLeg();
        m_Player.Motor.SetRestPosition();
        m_Player.LookAtCursor();
        m_Player.Animation.SetCapsulePosition(m_Player.PlayerInput.MoveInput);

        if (m_InputHandler.StretchInput)
        {
            m_StateMachine.ChangeState(m_Player.StretchState);
        }
        else if(m_InputHandler.MoveInput == Vector2.zero)
        {
            m_StateMachine.ChangeState(m_Player.IdleState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 finalForce = Vector2.zero;
        finalForce += m_Player.Motor.GetMoveForce(m_InputHandler.MoveInput);

        if (m_Player.Motor.IsPlayerAtMaxDistance())
        {
            finalForce += m_Player.Motor.RestrainToRestPosition();
        }

        m_Player.Motor.MovePlayer(finalForce);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }
}
