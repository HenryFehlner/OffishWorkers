using GameStateEnums;
using Godot;
using System;

public partial class StateManager : Node
{
    //fields
    private static GameState gameState = GameState.Gameplay;//default to gameplay for now

    //properties
    public static GameState CurrentState { get { return gameState; } }

    //methods
    public static void SetGameState(GameState state)
    {
        gameState = state;
        switch(state)
        {
            case GameState.Dead:
                
                break;
        }
    }
}
