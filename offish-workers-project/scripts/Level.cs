using Godot;
using System;
using System.Collections.Generic; 

public partial class Level : Node2D
{
	//PURPOSE OF LEVEL
	//storage for essential level information	

	//NEEDS
	/*
	List of enemies
	Tilemaps with completed level layout
	breakable items
	Entry and exit positions
	Hydration items
	*/
	
	[Export] protected Node mainEnemyContainer;
	private Node enemyContainer;
	private Node hydrationContainer; 
	
	public Godot.Collections.Array<Godot.Node> enemyContainerList;
	public List<Enemy> enemiesList;

	public Vector2 spawnPosition; 
	
	//Enemies List property
	public List<Enemy> EnemiesList
	{
		get { return enemiesList; }
		set { enemiesList = value; }
	}
	
	public Vector2 SpawnPosition
	{
		get { return spawnPosition; }
	}
	
	public override void _Ready()
	{
		//enemyContainer = GetNode<Node>("Enemy Container");
		enemiesList = new List<Enemy>(); 
		
		spawnPosition = new Vector2(0,0);
		
		AddEnemies(); 
	}
	
	////Adds the list of enemies in the enemy container
	//public void AddEnemies()
	//{
		//if(enemyContainer != null)
		//{
			//GD.Print("Looking for children");
			//Godot.Collections.Array<Node> children = enemyContainer.GetChildren(); 
	//
			//foreach (Node child in children)
			//{
				//GD.Print("Found children");
				//
				//if (child is CharacterBody2D enemyBody)
				//{
					//Enemy enemyScript = child as Enemy;
//
					//if (enemyScript != null)
					//{
						//GD.Print("Enemy script found on: " + child.Name);
						//enemiesList.Add(enemyScript);
					//}
					//else
					//{
						//GD.PrintErr("No Enemy script found on: " + child.Name);
					//}	
				//}
			//}
		//}
	//}
	
	//Adds the list of enemies in the enemy container
	public void AddEnemies()
	{
		if(mainEnemyContainer != null)
		{
			GD.Print("Looking for enemy containers");
			enemyContainerList = mainEnemyContainer.GetChildren();
	
			// Iterate through sub enemy containers
			foreach (Node container in enemyContainerList)
			{
				foreach (Node child in container.GetChildren())
				{
					if (child is CharacterBody2D enemyBody)
					{
						Enemy enemyScript = child as Enemy;

						if (enemyScript != null)
						{
							GD.Print("Enemy script found on: " + child.Name);
							enemiesList.Add(enemyScript);
						}
						else
						{
							GD.PrintErr("No Enemy script found on: " + child.Name);
						}	
					}
				}
			}
		}
	}
	
	public void UpdateEnemiesList()
	{
		enemiesList = new List<Enemy>(); 
		
		AddEnemies(); 
	}

}
