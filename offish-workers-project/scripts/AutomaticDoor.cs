using Godot;
using System;

public partial class AutomaticDoor : Node2D
{
	[Export] protected RigidBody2D doorRigidBody;
	[Export] protected Area2D closeTriggerArea;
	[Export] protected Sprite2D doorSprite;
	[Export] protected Node enemyContainer;
	private bool closed = false;
	private bool triggerable = true;
	
	public override void _Ready()
	{
		// Start with door inactive
		doorRigidBody.CollisionLayer = Layers.Bit(Layers.INACTIVE_DOORS);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (!closed && triggerable)
		{
			if (closeTriggerArea.HasOverlappingBodies())
			{
				// Activate door
				doorRigidBody.CollisionLayer = Layers.Bit(Layers.ENVIRONMENT);
				doorSprite.Visible = true;
				closed = true;
			}
		}
		else if (closed && triggerable)
		{
			if (enemyContainer.GetChildren().Count == 0)
			{
				// Deactivate door
				GD.Print("Opening door");
				doorRigidBody.CollisionLayer = Layers.Bit(Layers.INACTIVE_DOORS);
				doorSprite.Visible = false;
				closed = false;
				triggerable = false;	// no longer change state if closed and reopened
			}
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
