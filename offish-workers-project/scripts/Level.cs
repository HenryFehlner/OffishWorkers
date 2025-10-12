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
	
	private Node enemyContainer; 
	private Node hydrationContainer; 
	
	public List<Enemy> enemiesList; 
	
	//Enemies List property
	public List<Enemy> EnemiesList
	{
		get { return enemiesList; }
	}
	
	public override void _Ready()
	{
		enemyContainer = GetNode<Node>("Enemy Container");
		enemiesList = new List<Enemy>(); 
		
		AddEnemies(); 
	}
	
	//Adds the list of enemies in the enemy container
	public void AddEnemies()
	{
		if(enemyContainer != null)
		{
			GD.Print("Looking for children");
			Godot.Collections.Array<Node> children = enemyContainer.GetChildren(); 
	
			foreach (Node child in children)
			{
				GD.Print("Found children");
				
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
