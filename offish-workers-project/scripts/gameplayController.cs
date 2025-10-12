using Godot;
using System;
using System.Collections.Generic; 

public partial class gameplayController : Node2D
{
	//PURPOSE OF GAMEPLAY CONTROLLER
	//respawns and spawns enemies
	//loads levels
	//manages game state
	
	[Export] public CharacterBody2D player; 
	private Player playerScript; 

	[Export] private PackedScene currentLevel;
	[Export] private int currentLevelNumber; 
	[Export] private Level currentLevelScript;

	private List<Enemy> currentEnemiesList; 
	
	private Dictionary<string, PackedScene> enemyPrefabs; 
	private Dictionary<int, PackedScene> levelPrefabs; 
	
	public override void _Ready()
	{
		LoadLevel(0);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("debug_respawn"))
		{
			
		}
	}

	private void LoadLevel(int levelNum){
	//instantiate new level
	//set references to new level
	//including references to enemy list and such
	}
	
	private void NextLevelReached()
	{
		currentLevelNumber++; 
		//Delete old level
		//Call load level on new one
	}
	
	//Called when player dies
	private void OnPlayerDeath(){
		//Change gamestate to game over
	}
	
	public void RespawnEnemies()
	{
		foreach (Enemy enemyData in currentEnemiesList)
		{
			SpawnEnemy(enemyData);
		}
	}
	
	public void SpawnEnemy(Enemy enemyData)
	{
		
	}
}
