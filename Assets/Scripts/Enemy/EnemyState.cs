using UnityEngine;

public class EnemyState : State
{
    protected StateMachine m_StateMachine;
    protected Enemy m_Enemy;

    public EnemyState(StateMachine stateMachine, Enemy enemy) 
    {
        m_StateMachine = stateMachine;
        m_Enemy = enemy;
    }

   
}
