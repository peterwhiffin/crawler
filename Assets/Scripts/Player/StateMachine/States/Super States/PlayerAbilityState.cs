using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    protected bool m_IsAbilityDone = false;

    public PlayerAbilityState(StateMachine stateMachine, Player player, InputHandler inputHandler) : base(stateMachine, player, inputHandler)
    {
    }

    public override void Enter()
    {
        base.Enter();
        m_IsAbilityDone = false;
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
    }
}
