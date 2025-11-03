using Godot;
using System;

public partial class hydrationRestoreObject : Area2D
{	
	//Two different types of objects 
	//True => restores health over time and stops hydration from ticking down
	//False => restores in one big burst, disappears when "collected"
	[Export] protected bool isReusable = true;
	
	//Restore amount should be very small if "reusable"
	[Export] protected int restoreAmount = 3; 
	
	//Is the restore object a blood puddle from bloodbath, false default for now
	[Export] protected bool isBlood = false;
	
	//How long in between each restore tick for reusable puddles
	private Timer restoreTimer; 
	
	//True when it is time to heal the player -> I can definitely implement this better but idc rn 
	private bool timeToHeal = false;
	
	//Storage for player's Player.cs
	private Player playerScript;
	
	public override void _Ready()
	{
		base._Ready();

		if (isBlood)
		{
			isReusable = false;
		}

		CollisionLayer = Layers.Bit(Layers.ENVIRONMENT);
		CollisionMask = Layers.Bit(Layers.PLAYER) | Layers.Bit(Layers.DODGE);

		//Setup all timers and signal events
		restoreTimer = GetNode<Timer>("RestoreTimer");
		restoreTimer.Timeout += RestoreOnTimeout; 
		restoreTimer.WaitTime = 1.0f;
		
		this.BodyEntered += OnBodyEntered; 
		this.BodyExited += OnBodyExited;
		
	}
	
	//Checks if it needs to heal every frame
	public override void _PhysicsProcess(double delta)
	{
		if (timeToHeal == true)
		{
			if (playerScript != null)
			{
				playerScript.RestoreHydration(restoreAmount);
				timeToHeal = false;
			}
		}
	}
	
	//Restores a player's health once they enter the object
	//and disappears if it isn't tagged as reusable
	private void OnBodyEntered(Node body)
	{				
		if (body is CharacterBody2D player)
		{
			//GD.Print("Player entered restore object");
			playerScript = player as Player; 
			
			if (isReusable == false)
			{
			
				if (playerScript != null)
				{
					playerScript.RestoreHydration(restoreAmount);
				}
				
				RemoveSelf(); 
			}
			else 
			{
			//GD.Print("In reusable object");
				restoreTimer.Start(); 
				playerScript.OnHydrationRestore = true; 
			}
		}
	}
	
	private void OnBodyExited(Node body)
	{
		if (body is CharacterBody2D player){
			restoreTimer.Stop();
			//GD.Print("Player left restore object"); 
			playerScript.OnHydrationRestore = false; 
		}
	}
	
	//Deletes self if it is only collectable once
	private void RemoveSelf(){
		//GD.Print("Hydration absorbed");
		QueueFree(); 
	}
	
	//Called every time the restoration timer times out for
	//fish standing on the puddle and healing over time
	private void RestoreOnTimeout()
	{
		timeToHeal = true; 
	}
}
