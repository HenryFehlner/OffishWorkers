using Godot;
using System;

public partial class exitTile : CollisionShape2D
{
	[Signal] public delegate void EndTileReachedEventHandler();
	
	private gameplayController gameplayControllerScript; 
	
	public override void _Ready()
	{
		if (gameplayControllerScript == null)
		{
		gameplayControllerScript = GetNode("../../Gameplay Controller") as GameplayController;
		}
	}
		
	//Sends the end tile event reached signal once the player enters it
	private void OnBodyEntered(Node body)
	{				
		if (body is CharacterBody2D player)
		{
			ChangeLevel(); 
		}
	}
	
	private void ChangeLevel()
	{
		
	}
}
