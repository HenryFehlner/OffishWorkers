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
	
	[Export] public Player player; 
	

	private PackedScene currentLevel;//current level should be set in the loadLevel methods
	private Node2D currentLevelNode;
	[Export] private int currentLevelNumber;
	
	//Number of levels we have implemented
	private int levelAmount = 2;
	
	[Export] private Level currentLevelScript;

	private List<Enemy> currentEnemiesList; 
	private List<Enemy> replacementEnemiesList; 
	
	
	private Dictionary<string, PackedScene> enemyPrefabs = new Dictionary<string, PackedScene>();
	private Dictionary<int, PackedScene> levelPrefabs = new Dictionary<int, PackedScene>();//need to set up

	public override void _Ready()
	{
		LoadEnemyScenes();
		LoadLevelInfo();
		
		//currentLevelScript = GetNode("../Level Container/Level") as Level;
		//player = GetNode<CharacterBody2D>("../Player");
		
		currentLevelNumber = 0;
		//load first level
		LoadFirstLevel();
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

	private void LoadFirstLevel()
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
		
		currentLevelScript = LoadLevelScene(); 	
		//playerScript.MoveTo(currentLevelScript.SpawnPosition);
		player.MoveTo(Vector2.Zero);
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
		
		//i dont think this is neccessary, the whole level is unloaded
		// if(currentLevelScript.EnemiesList!=null)
		// {
		// 	currentEnemiesList = currentLevelScript.EnemiesList; 
			
		// 	foreach (Enemy enemyData in currentEnemiesList)
		// 	{
		// 		CharacterBody2D enemyBody = (CharacterBody2D)enemyData.GetParent();
		// 		enemyBody.QueueFree(); 
		// 	}
		// }

		currentLevelScript = LoadLevelScene(); 	
		player.MoveTo(currentLevelScript.SpawnPosition);
	}
	
	//Loads in the level from the packed scene dictionary 
	//Called when actually instantiating the new scene
	private Level LoadLevelScene()
	{
		currentLevel = levelPrefabs[currentLevelNumber];

		//remove old level
		if(currentLevelNode != null && currentLevelNode.IsInsideTree())
		{
			currentLevelNode.QueueFree();
		}		

		Node2D levelInstance = (Node2D)currentLevel.Instantiate(); 
		GetNode<Node>("../Level Container").AddChild(levelInstance);

		//need to give each enemy a ref to the player
		//enemy.SetPlayer(player)

		currentLevelNode = levelInstance;
		
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
		//the first level is index 1, second is index 2, and so on
		levelPrefabs[1] = GD.Load<PackedScene>("res://scenes/Levels/tutorial.tscn");
		levelPrefabs[2] = GD.Load<PackedScene>("res://scenes/Levels/Sprint3Level.tscn");
	}
	
	//Called when player dies
	private void OnPlayerDeath()
	{
		//Change gamestate to game over
	}
	
	private void LoadEnemyPrefabs()
	{
		
	}
	
	// This probably doesn't work anymore with new sub enemy containers
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
		
		return newEnemyScript; 
	}
}
