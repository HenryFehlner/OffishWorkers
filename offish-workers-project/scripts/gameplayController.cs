using Godot;
using System;
using System.Collections.Generic; 
using GameStateEnums; 

public partial class gameplayController : Node2D
{
	//PURPOSE OF GAMEPLAY CONTROLLER
	//respawns and spawns enemies
	//loads levels
	//manages game state
	
	private GameState currentGameState; 
	
	[Export] public CharacterBody2D player; 
	private Player playerScript; 
	

	[Export] private PackedScene currentLevel;
	[Export] private int currentLevelNumber; 
	
	//Number of levels we have implemented
	private int levelAmount = 1;
	
	[Export] private Level currentLevelScript;

	private List<Enemy> currentEnemiesList; 
	private List<Enemy> replacementEnemiesList; 
	
	
	private Dictionary<string, PackedScene> enemyPrefabs = new Dictionary<string, PackedScene>(); 
	private Dictionary<int, PackedScene> levelPrefabs = new Dictionary<int, PackedScene>();
	
	public override void _Ready()
	{
		LoadEnemyScenes(); 
		
		currentLevelScript = GetNode("../Level Container/Level") as Level;
		player = GetNode<CharacterBody2D>("../Player");
		
		currentLevelNumber = 0; 
	}
	
	public override void _PhysicsProcess(double delta)
	{
		switch (currentGameState)
		{
			case GameState.GamePlay: 					
				if (Input.IsActionJustPressed("debug_respawn"))
				{
					RespawnEnemies(); 
				}
				break; 
			case GameState.PauseMenu: 
				break;
			case GameState.Upgrade:
				break; 
		}
	}

	//Loads in a new level when the end tile of a level is reached
	public void LoadNextLevel()
	{
		//Increase level number
		//If the level number > level amount, the game is over
		//Destroy all of the enemies
		//Destroy the current scene
		//Load new scene in 
		currentLevelNumber++; 
		
		if (currentLevelNumber > levelAmount)
		{
			currentGameState = GameState.Finished; 
			return; 
		}
		
		currentEnemiesList = currentLevelScript.EnemiesList; 
		
		foreach (Enemy enemyData in currentEnemiesList)
		{
			CharacterBody2D enemyBody = (CharacterBody2D)enemyData.GetParent();
			enemyBody.QueueFree(); 
		}

		currentLevelScript = LoadLevelScene(); 	
		playerScript.MoveTo(currentLevelScript.SpawnPosition);
	}
	
	//Loads in the level from the packed scene dictionary 
	//Called when actually instantiating the new scene
	private Level LoadLevelScene()
	{
		currentLevel = levelPrefabs[currentLevelNumber];
		Node2D levelInstance = (Node2D)currentLevel.Instantiate(); 
		GetNode<Node2D>("../Level Container").AddChild(levelInstance);
		
		Level newLevelScript = levelInstance as Level;
		
		return newLevelScript; 
	}
	

	//Only touch this when we have new enemies to add
	private void LoadEnemyScenes()
	{
		enemyPrefabs["punchingBag"] = GD.Load<PackedScene>("res://scenes/Enemies/BaseEnemy.tscn");
	}
	
	//Only touch this when we have new levels to add
	private void LoadLevelInfo()
	{
		levelPrefabs[0] = GD.Load<PackedScene>("res://scenes/Levels/Sprint3Level.tscn");
	}
	
	//Called when player dies
	private void OnPlayerDeath()
	{
		//Change gamestate to game over
	}
	
	private void LoadEnemyPrefabs()
	{
		
	}
	
	//Called to respawn all enemies in a level
	public void RespawnEnemies()
	{
		replacementEnemiesList = new List<Enemy>();   
		List<Enemy> newEnemiesList = new List<Enemy>(); 
		
		currentEnemiesList = currentLevelScript.EnemiesList; 
		
		foreach (Enemy enemyData in currentEnemiesList)
		{
			
			if (enemyData.IsDead == true)
			{
			Enemy newEnemy = SpawnEnemy(enemyData);
			replacementEnemiesList.Add(newEnemy);
			}
			else 
			{
			replacementEnemiesList.Add(null);
			}
		}
		
		foreach (Enemy enemyData in currentEnemiesList)
		{
			int index = currentEnemiesList.IndexOf(enemyData);
			
			if (replacementEnemiesList[index] != null)
			{
				newEnemiesList.Add(replacementEnemiesList[index]);
			}
			else 
			{
				newEnemiesList.Add(enemyData);
			}
		}
		
		currentLevelScript.EnemiesList = newEnemiesList; 
	}
	
	//Only call this when spawning a new version of enemies
	//They should come prespawned with the level
	private Enemy SpawnEnemy(Enemy enemyData)
	{
		PackedScene enemyScene = enemyPrefabs[enemyData.EnemyType];
		CharacterBody2D enemyInstance = (CharacterBody2D)enemyScene.Instantiate(); 
		GetNode<Node2D>("../Level Container/Level/Enemy Container").AddChild(enemyInstance);
		enemyInstance.GlobalPosition = enemyData.spawnPosition; 
		
		Enemy newEnemyScript = enemyInstance as Enemy; 
		GD.Print("Enemy Script: " + newEnemyScript);
		newEnemyScript.SetPlayer(player);
		
		return newEnemyScript; 
	}
	

}
