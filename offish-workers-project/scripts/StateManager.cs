using GameStateEnums;
using Godot;
using System;

public partial class StateManager : Node
{
	//fields
	private static GameState currentState = GameState.MainMenu;//default to gameplay for now
	//private static GameState menuState = GameState.MainMenu; //Starting point
	//properties
	public static GameState CurrentState { 
		get => currentState;
		set => currentState = value;
	}

	//methods
	public static void SetGameState(GameState state)
	{
		currentState = state;
		//to check if it runs correctly
		GD.Print("Changing state to: " + state);
		switch(state)
		{
			case GameState.MainMenu:
				//Loads mainmenu
				//GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
				break;
			case GameState.GamePlay:
				//GetTree().ChangeSceneToFile("res://scenes/main.tscn");
				break;
			case GameState.PauseMenu:
				//Load Pause Menu
				break;
			case GameState.Upgrade:
				//Load Upgrades
				break;
			case GameState.Finished:
				//Load Endgame
				break;
			case GameState.Dead:
				
				break;
		}
	}
}
