using Godot;
using System;

public abstract partial class BaseEnemy : RigidBody2D
{
	//Current amount of health the enemy has
	[Export]
	protected int currentHealth;
	
	//Maximum health the enemy 
	[Export]
	protected int maxHealth; 
	
	//Damage dealt when the enemy hits the player
	[Export]
	protected int attackDamage;
	
	//Movement speed of the enemy, done so that it is easily scalable. 
	[Export]
	protected float movementSpeed; 
	
	//Tracks if the enemy is dead
	[Export]
	private bool isDead = true; 
	
	
	//Called whenever the enemy takes damage, reduces the current health
	//by the amount of damage done
	protected void TakeDamage(int damage)
	{
		currentHealth -= damage; 
	}
	
	//Called to check if the enemy is dead
	protected void CheckIfDead()
	{
		if (currentHealth <= 0)
		{
			isDead = true;
		}
	}
	
	//Attack method to be implemented
	protected abstract void Attack(); 
	
	
}
