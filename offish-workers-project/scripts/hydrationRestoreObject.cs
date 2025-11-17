using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class hydrationRestoreObject : Area2D
{	
	//Two different types of objects 
	//True => restores health over time and stops hydration from ticking down
	//False => restores in one big burst, disappears when "collected"
	[Export] protected bool isReusable = true;
	
	//Restore amount should be very small if "reusable"
	[Export] protected int restoreAmount = 3; 
	
	// For reusable pools
	[Export] protected float continuousRestoreRate = 8.0f;
	
	//Is the restore object a blood puddle from bloodbath, false default for now
	[Export] protected bool isBlood = false;
	
	//How long in between each restore tick for reusable puddles
	private Timer restoreTimer; 
	
	//True when it is time to heal the player -> I can definitely implement this better but idc rn 
	private bool timeToHeal = false;
	
	//Storage for player's Player.cs
	private Player playerScript;
	
	public static hydrationRestoreObject Create(
		bool isReusable = false,
		int restoreAmount = 30,
		string filePath = "res://scenes/Other Entities/hydrationRestoreObjectSingleUse.tscn"
		)
    {
		//create object
		PackedScene prefab = GD.Load<PackedScene>(filePath);
        hydrationRestoreObject obj = prefab.Instantiate<hydrationRestoreObject>();

		//set values
		obj.isReusable = isReusable;
		obj.restoreAmount = restoreAmount;
		
		//return object
		return obj;
    }

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
		// Continuous healing if reusable
		if (playerScript != null && isReusable)
		{
			var overlappingBodies = GetOverlappingBodies();
			
			if (overlappingBodies.Count != 0)
			{
				playerScript.IsHealing = true;
			}
		}
	}
	
	//Restores a player's health once they enter the object
	//and disappears if it isn't tagged as reusable
	private void OnBodyEntered(Node body)
	{
		// Single use restore
		if (body is CharacterBody2D player)
		{
			playerScript = player as Player;
			
			if (isReusable == false && playerScript != null)
			{
				playerScript.RestoreHydrationOnce(restoreAmount);
				RemoveSelf(); 
			}
		}
	}
	
	private void OnBodyExited(Node body)
	{
		// Set stop healing and set player script to null
		if (body is CharacterBody2D player){
			//GD.Print("Player left restore object"); 
			//restoreTimer.Stop();
			//playerScript.OnHydrationRestore = false;
			
			playerScript.IsHealing = false;
			playerScript = null;
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
