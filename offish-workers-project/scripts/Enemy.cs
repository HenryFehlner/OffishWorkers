using Godot;
using System;

public partial class Enemy : CharacterBody2D
{	
	[Export] protected int maxSpeed = 1;
	[Export] protected int acceleration = 1;
	[Export] protected int friction = 20;
	[Export] protected int maxHp = 100;
	[Export] protected float knockbackMultiplier = 1.0f;
	[Export] protected int detectionRadius = 500;
	protected int currentHp;
	private CharacterBody2D _player;
	
	[Export] public Vector2 spawnPosition; 
	[Export] private string enemyType; 
	
	public string EnemyType
	{
		get { return enemyType; }
	}

	public void SetPlayer(CharacterBody2D player)
	{
		_player = player;
		if (player == null) 
		 GD.Print("Can't find player");
	}

	public override void _Ready()
	{
		base._Ready();
		
		//add to group for registering attacks
		AddToGroup("enemies");
		//set collision layer and masks
		CollisionLayer = Layers.Bit(Layers.ENEMIES);
		CollisionMask = Layers.Bit(Layers.ENVIRONMENT) | Layers.Bit(Layers.PLAYER_ATTACKS);

		if (enemyType == null)
		{
			enemyType = "punchingBag";
		}
		spawnPosition = this.Position; 
		currentHp = maxHp;

	}

	public override void _PhysicsProcess(double delta)
	{
		//movement
		Move(delta);
	}

	protected virtual void Move(double delta)
	{
		if(_player == null)
		{
			 return;
		}
		// Get the direction from enemy to player
		Vector2 direction = (_player.GlobalPosition - this.GlobalPosition);
		// Only move towards the player if the player is within the detection radius
		if(GlobalPosition.DistanceTo(_player.GlobalPosition) < detectionRadius)
		{
			// Calculate velocity, accounting for acceleration and max speed
			Velocity = Velocity.Lerp(direction * maxSpeed, (float)delta * acceleration);
			// Apply Friction
			Velocity = Velocity.MoveToward(Vector2.Zero, friction * (float)delta);
		}
		else
		{
			Velocity = Vector2.Zero;
		}
		MoveAndSlide();
	}


	/// <summary>
	/// Take a hit from an attack, receiving damage and knockback
	/// </summary>
	/// <param name="damage">damage</param>
	/// <param name="impulse">knockback force</param>
	/// <param name="attacker">source of the attack</param>
	public void TakeHit(int damage, Vector2 impulse, Node attacker)
	{
		//prevent negative damage from healing
		damage = Math.Max(damage, 0);
		//reduce health
		currentHp -= damage;
		GD.Print($"Enemy took {damage} damage from {attacker}! Enemy has {currentHp} HP remaining!");
		//apply knockback
		GD.Print($"{impulse * knockbackMultiplier}");
		Velocity += impulse * knockbackMultiplier;
		//check for death
		if (currentHp <= 0)
		{
			//delete enemy
			QueueFree();
		}
	}

}
