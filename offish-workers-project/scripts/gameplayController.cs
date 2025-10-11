using Godot;
using System;
using System.Collections.Generic; 

public partial class gameplayController : Node2D
{
	[Export] public CharacterBody2D player; 
	private Player playerScript; 
	[Export] public PackedScene enemyDebug; 

	public List<PackedScene> levelList; 
	[Export] private PackedScene currentLevel;
	
	public override void _Ready()
	{
		if (Input.IsActionJustPressed("debug_respawn"))
		{
			
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		
	}

	private void LoadLevel(){
		//Need to set up new level and get rid of old one
		//
	}
	
	//Called when player dies
	private void OnPlayerDeath(){
		//
	}
}
