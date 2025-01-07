using UnityEngine;

public class PlayerJumpState : PlayerAbilityState
{
    public PlayerJumpState(StateMachine stateMachine, Player player, InputHandler inputHandler) : base(stateMachine, player, inputHandler)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        //m_InputHandler.JumpInput = false;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!m_IsAbilityDone)
        {
            var direction = m_Player.Motor.GetJumpDirection();

            if(m_InputHandler.MoveInput != Vector2.zero)
            {
                direction += (Vector2.up * m_InputHandler.MoveInput.y) + (Vector2.right * m_InputHandler.MoveInput.x);

              
            }

            direction = direction.normalized;

            m_Player.Motor.LaunchPlayer();
            m_Player.Motor.MovePlayer(direction * m_Player.CrawlerSettings.JumpForce, ForceMode2D.Impulse);
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
