using Godot;
using System;

public partial class gameplayController : Node2D
{
	[Export] CharacterBody2D player; 
	[Export] Player playerScript; 

	var currentLevel;
	

	private void LoadLevel(){
		//Need to set up new level and get rid of old one
	}
	
	//Called when player dies
	private void OnPlayerDeath(){
		
	}
}
