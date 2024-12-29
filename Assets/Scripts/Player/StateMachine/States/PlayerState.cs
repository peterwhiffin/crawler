using UnityEngine;

public class PlayerState
{
    protected StateMachine m_StateMachine;
    protected Player m_Player;
    protected InputHandler m_InputHandler;

    public PlayerState(StateMachine stateMachine, Player player, InputHandler inputHandler)
    {
        m_StateMachine = stateMachine;
        m_Player = player;
        m_InputHandler = inputHandler;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void LateUpdate() { }    
}
