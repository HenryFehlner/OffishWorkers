using Godot;
using System;

public partial class AutomaticDoor : Node2D
{
	[Export] private RigidBody2D doorRigidBody;
	[Export] private Area2D closeTriggerArea;
	[Export] private Sprite2D doorSprite;
	[Export] private Node enemyContainer;
	
	[Export] private bool closed = false;	// Determines the starting state of the door
	private bool triggerable = true;		// Allows the door to only close and reopen once
	
	public override void _Ready()
	{
		// Start with door inactive
		doorRigidBody.CollisionLayer = Layers.Bit(Layers.INACTIVE_DOORS);
		
		if (closed)
		{
			CloseDoor();
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		// Check for player entering the close trigger
		if (!closed && triggerable)
		{
			if (closeTriggerArea.HasOverlappingBodies())
			{
				// Activate door
				//doorRigidBody.CollisionLayer = Layers.Bit(Layers.ENVIRONMENT);
				//doorSprite.Visible = true;
				//closed = true;
				
				CloseDoor();
			}
		}
		// Check for the assigned enemy container to be empty
		else if (closed && triggerable)
		{
			if (enemyContainer.GetChildren().Count == 0)
			{
				// Deactivate door
				GD.Print("Opening door");
				doorRigidBody.CollisionLayer = Layers.Bit(Layers.INACTIVE_DOORS);
				doorSprite.Visible = false;
				closed = false;
				triggerable = false;
				GD.Print(doorRigidBody.CollisionLayer);
			}
		}
	}
	
	private void CloseDoor()
	{
		doorRigidBody.CollisionLayer = Layers.Bit(Layers.ENVIRONMENT);
		doorSprite.Visible = true;
		closed = true;
	}
}
