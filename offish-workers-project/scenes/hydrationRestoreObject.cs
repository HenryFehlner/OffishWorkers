using Godot;
using System;

public partial class hydrationRestoreObject : Area2D
{
	[Export] protected int restoreAmount = 3; 
	[Export] protected bool isReusable = true;
	[Export] protected bool isBlood; 
	
	//How long in between each restore tick
	private Timer restoreTimer; 
	
	//True when it is time to heal the player -> I can definitely implement this better but idc rn 
	private bool timeToHeal = false;
	
	//Storage for player's Player.cs
	Player playerScript;
	
	public override void _Ready()
	{
		base._Ready(); 
		
		if (isBlood)
		{
			isReusable = false;
		}
		
		restoreTimer = GetNode<Timer>("RestoreTimer");
		restoreTimer.Timeout += RestoreOnTimeout; 
		restoreTimer.WaitTime = 1.0f;
		this.BodyEntered += OnBodyEntered; 
		this.BodyExited += OnBodyExited;
		
	}
	
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
			GD.Print("Player entered restore object");
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
			GD.Print("In reusable object");
				restoreTimer.Start(); 
				playerScript.OnHydrationRestore = true; 
			}
		}
	}
	
	private void OnBodyExited(Node body)
	{
		if (body is CharacterBody2D player){
			restoreTimer.Stop();
			GD.Print("Player left restore object"); 
			playerScript.OnHydrationRestore = false; 
		}
	}
	
	//Deletes self if it is only collectable once
	private void RemoveSelf(){
		GD.Print("Hydration absorbed");
		QueueFree(); 
	}
	
	//Called every time the restoration timer times out for
	//fish standing on the puddle and healing over time
	private void RestoreOnTimeout()
	{
		timeToHeal = true; 
	}
}
