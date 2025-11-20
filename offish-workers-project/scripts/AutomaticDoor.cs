using Godot;
using System;

public partial class AutomaticDoor : Node2D
{
	[Export] protected RigidBody2D doorRigidBody;
	[Export] protected Area2D closeTriggerArea;
	[Export] protected Sprite2D doorSprite;
	[Export] protected Node enemyContainer;
	private bool closed = false;
	
	public override void _Ready()
	{
		// Start with door inactive
		doorRigidBody.CollisionLayer = Layers.Bit(Layers.INACTIVE_DOORS);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		// Check if player enters close trigger
		if (!closed && closeTriggerArea.HasOverlappingBodies())
		{
			// Activate door collision
			doorRigidBody.CollisionLayer = Layers.Bit(Layers.ENVIRONMENT);
			doorSprite.Visible = true;
			closed = true;
		}
		else if (closed && enemyContainer.GetChildren().Count == 0)
		{
			GD.Print("Reopen door");
		}
	}
	
	//private void OnBodyEntered(Node body)
	//{
		//// Check for player in the close trigger
		//if (body is CharacterBody2D player)
		//{
			//GD.Print("player in area");
		//}
	//}
}
