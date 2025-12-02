using Godot;
using System;

public partial class Main : Node2D
{
	public override void _Ready()
	{
		// Get the player node
		CharacterBody2D player = GetNode<CharacterBody2D>("Player");

		// Get all enemies in the "enemies" group
		var enemies = GetTree().GetNodesInGroup("enemies");

		// foreach (Node enemyNode in enemies)
		// {
		// 	Enemy enemyScript = enemyNode as Enemy;
		// 	if (enemyScript != null)
		// 	{
		// 		enemyScript.SetPlayer(player);
		// 	}
		// }   
	}
	
}
