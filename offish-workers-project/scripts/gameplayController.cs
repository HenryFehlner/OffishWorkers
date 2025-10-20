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
	
	//Number of levels we have implemented
	private int levelAmount = 1;
	
	[Export] private Level currentLevelScript;

	private List<Enemy> currentEnemiesList; 
	private List<Enemy> replacementEnemiesList; 
	
	private Dictionary<string, PackedScene> enemyPrefabs = new Dictionary<string, PackedScene>(); 
	private Dictionary<int, PackedScene> levelPrefabs = new Dictionary<int, PackedScene>();
	
	public override void _Ready()
	{
		LoadEnemyInfo(); 
		
		currentLevelScript = GetNode("../Level") as Level;
		player = GetNode<CharacterBody2D>("../Player");
		
		LoadTestLevel(); 
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("debug_respawn"))
		{
			RespawnEnemies(); 
		}
	}

	private void LoadLevel(int levelNum)
	{
		currentEnemiesList = currentLevelScript.EnemiesList; 
	
	}
	
	private void LoadTestLevel(){
		
	}
	
	//Only touch this when we have new enemies to add
	private void LoadEnemyInfo()
	{
		enemyPrefabs["punchingBag"] = GD.Load<PackedScene>("res://scenes/Enemies/BaseEnemy.tscn");
	}
	
	//Only touch this when we have new levels to add
	private void LoadLevelInfo(){
		//levelPrefabs[0] = GD.Load<PackedScene>("res://scenes/levels/levelX.tscn");
	}
	private void NextLevelReached()
	{
		currentLevelNumber++; 
		
		//Delete old level
		//Call load level on new one
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
		GetNode<Node2D>("../Level/Enemy Container").AddChild(enemyInstance);
		enemyInstance.GlobalPosition = enemyData.spawnPosition; 
		
		Enemy newEnemyScript = enemyInstance as Enemy; 
		GD.Print("Enemy Script: " + newEnemyScript);
		newEnemyScript.SetPlayer(player);
		
		return newEnemyScript; 
	}
	

}
