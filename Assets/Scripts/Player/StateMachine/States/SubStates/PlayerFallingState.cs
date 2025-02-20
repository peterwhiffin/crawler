using UnityEngine;

public class PlayerFallingState : PlayerInAirState
{
    public PlayerFallingState(StateMachine stateMachine, Player player, InputHandler inputHandler) : base(stateMachine, player, inputHandler)
    {
    }

    private float m_TimeEntered;
    private bool m_Land = false;

    public override void Enter()
    {
        base.Enter();
        m_TimeEntered = Time.time;
        m_Player.Motor.PlayerInAir();
        m_Land = false;
    }

    public override void Exit()
    {
        base.Exit();
        m_Player.Motor.EndLaunch();
    }

    public override void Update()
    {
        base.Update();
        m_Player.Motor.CheckLegsInAir();
        m_Player.Motor.SetRestPosition();
        m_Player.LookAtCursor();
        m_Player.Animation.SetCapsulePosition(m_Player.PlayerInput.MoveInput);


        if (!m_InputHandler.JumpInput)
        {
            m_Land = true;
        }


        if (m_Land && m_Player.Motor.CanPlayerLand(m_TimeEntered))
        {
            m_Player.Motor.CheckLegsWithoutPosition();
            m_Player.Motor.CheckCurrentLeg();

            if (m_InputHandler.JumpInput && m_Player.Motor.IsGrappled || m_Player.Motor.m_ReelingObject)
            {
                m_StateMachine.ChangeState(m_Player.GrappleState);
            }
            else if (m_Player.Motor.EnoughLegsToWalk())
            {
                if (m_Player.Motor.IsGrappled)
                {
                    m_Player.Hotbar.ReelGrapple();
                }

                m_StateMachine.ChangeState(m_Player.IdleState);
            }

        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Vector2 finalForce = Vector2.zero;

        float moveSpeed = m_Player.CrawlerSettings.InAirMoveSpeed;

        if (m_Player.Motor.IsGrappled)
        {
            finalForce += m_Player.Motor.GetGrappleConstraint();
            moveSpeed = m_Player.CrawlerSettings.GrappleInAirMoveSpeed;
        }

        if (m_InputHandler.MoveInput != Vector2.zero)
        {
            var direction = (Vector2.right * m_InputHandler.MoveInput.x);
            direction = direction.normalized;

            finalForce += direction * moveSpeed;
        }

        

        m_Player.Motor.MovePlayer(finalForce);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();

    }
}
