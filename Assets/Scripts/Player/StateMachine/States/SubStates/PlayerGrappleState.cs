using UnityEngine;

public class PlayerGrappleState : PlayerAbilityState
{
    public PlayerGrappleState(StateMachine stateMachine, Player player, InputHandler inputHandler) : base(stateMachine, player, inputHandler)
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

        if (!m_IsAbilityDone)
        {
            m_Player.Motor.LaunchPlayer();
            m_Player.Motor.MovePlayer(m_Player.Motor.GetGrappleForce(), ForceMode2D.Impulse);
            m_Player.Hotbar.ReelGrapple();
            m_IsAbilityDone = true;
        }
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void Update()
    {
        base.Update();

        if (m_IsAbilityDone)
        {
            m_StateMachine.ChangeState(m_Player.FallingState);
        }
    }
}
