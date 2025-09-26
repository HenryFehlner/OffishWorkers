using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	protected int maxSpeed = 600;
	
	[Export]
	protected int acceleration = 10;
	
	[Export]
	protected int friction = 20;
	
	public override void _PhysicsProcess(double delta)
	{
		// Called every frame. Delta is time since the last frame.
		// Update game logic here.
		movePlayer(delta);
	}
	
	private void movePlayer(double delta)
	{
		// Get input
		Vector2 moveDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		
		// Apply acceleration
		if (moveDirection != Vector2.Zero)
		{
			Velocity = Velocity.Lerp(moveDirection * maxSpeed, (float)delta * acceleration);
		}
		// Apply friciton
		else
		{
			Velocity = Velocity.Lerp(Vector2.Zero, (float)delta * friction);
		}
		
		//GD.Print(Velocity);
		MoveAndSlide();
	}
}
