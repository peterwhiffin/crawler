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
        //if (Time.time - m_TimeEntered > m_Player.Motor.GetLaunchTime() && m_Player.Motor.IsPlayersVelocityBelowLandingThreshold())
        //{

        //    m_Player.Motor.CheckLegsWithoutPosition();
        //    m_Player.Motor.CheckCurrentLeg();

        //    if (m_Player.Motor.EnoughLegsToWalk())
        //    {
        //        m_StateMachine.ChangeState(m_Player.IdleState);
        //    }

        //}

        //if (m_Player.Motor.CanPlayerLand(m_TimeEntered))
        //{
        //    m_Player.Motor.CheckLegsWithoutPosition();
        //    m_Player.Motor.CheckCurrentLeg();

        //    if (m_Player.Motor.EnoughLegsToWalk())
        //    {
        //        m_StateMachine.ChangeState(m_Player.IdleState);
        //    }

        //}

        if (m_InputHandler.JumpInput)
        {
            m_Land = true;
        }


        if (m_Player.Motor.IsGrappled)
        {
            m_StateMachine.ChangeState(m_Player.GrappleState);
        }
        else if (m_Land)
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
        Vector2 finalForce = Vector2.zero;

        if(m_InputHandler.MoveInput != Vector2.zero)
        {
            var direction = (Vector2.right * m_InputHandler.MoveInput.x);
            direction = direction.normalized;

            finalForce += direction * m_Player.CrawlerSettings.InAirMoveSpeed;
        }

        m_Player.Motor.MovePlayer(finalForce);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }
}
