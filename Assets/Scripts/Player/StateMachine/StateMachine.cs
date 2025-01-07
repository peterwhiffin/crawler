using UnityEngine;
using System;

public class StateMachine
{
    public State CurrentState {  get; private set; }
    public Action StateChanged = delegate { };

    public void Initialize(State startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(State newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
        StateChanged.Invoke();
    }
}
