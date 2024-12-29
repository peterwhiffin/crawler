using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(StateMachine stateMachine, Player player, InputHandler inputHandler) : base(stateMachine, player, inputHandler)
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
        m_Player.Motor.CheckLegsWithoutPosition();
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
