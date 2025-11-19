using Godot;
using System;

public partial class AutomaticDoor : Node2D
{
	[Export] protected RigidBody2D doorRigidBody;
	//[Export] protected CollisionShape2D doorCollisionShape;
	[Export] protected Area2D closeTriggerArea;
	
	public override void _Ready()
	{
		// Start with door inactive
		//doorRigidBody.CollisionMask = Layers.Bit(Layers.ENVIRONMENT);
		//doorCollisionShape.SetDeferred(doorCollisionShape.PropertyName.Disabled, true);
		//doorCollisionShape.Disabled = true;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		//if (doorCollisionShape.Disabled == true && closeTriggerArea.HasOverlappingBodies())
		if (closeTriggerArea.HasOverlappingBodies())
		{
			// Activate door collision
			GD.Print("player entered door trigger");
			//doorCollisionShape.SetDeferred(doorCollisionShape.PropertyName.Disabled, false);
			//doorCollisionShape.Disabled = false;
			
			//doorRigidBody.CollisionMask = Layers.Bit(Layers.ENVIRONMENT | Layers.PLAYER | Layers.ENEMIES | Layers.DODGE);
			doorRigidBody.CollisionMask = Layers.Bit(Layers.DOORS);
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
