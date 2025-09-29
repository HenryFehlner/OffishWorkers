using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
	//movement
	[Export] protected int maxSpeed = 600;
	[Export] protected int acceleration = 10;
	[Export] protected int friction = 20;
	//stats
	[Export] protected int maxHp = 100;
	protected int currentHp;
	//primary attack
	[Export] protected int primaryDamage = 1;
	[Export] protected float primaryDuration = .4f;
	[Export] protected float primaryKnockbackAmount = 4000;


	private bool isAttacking = false;
	private Vector2 facingDirection = Vector2.Right; //default value so player can never face Vector2.zero

	public override void _Ready()
	{
		base._Ready();
		//add to group for registering attacks
		AddToGroup("player");
		//set collision layer and masks
		CollisionLayer = Layers.Bit(Layers.PLAYER);
		CollisionMask = Layers.Bit(Layers.ENVIRONMENT) | Layers.Bit(Layers.ENEMIES) | Layers.Bit(Layers.ENEMY_ATTACKS);

		currentHp = maxHp;
	}


	public override void _PhysicsProcess(double delta)
	{
		// Called every frame. Delta is time since the last frame.
		// Update game logic here.

		//check for attacks
		if (Input.IsActionJustPressed("primary_attack"))
		{
			_ = PrimaryAttack();
		}

		//movement
		MovePlayer(delta);
	}

	private void MovePlayer(double delta)
	{
		// Get input
		Vector2 moveDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");

		// Apply acceleration
		if (moveDirection != Vector2.Zero && !isAttacking)
		{
			Velocity = Velocity.Lerp(moveDirection * maxSpeed, (float)delta * acceleration);
			facingDirection = moveDirection.Normalized();
		}
		// Apply friciton
		else
		{
			Velocity = Velocity.Lerp(Vector2.Zero, (float)delta * friction);
		}

		//GD.Print(Velocity);
		MoveAndSlide();
	}

	private async Task PrimaryAttack()
	{
		if (isAttacking)
		{
			return;
		}
		isAttacking = true;

		//to have attacking move the player, add an impulse here

		RectangleShape2D rect = new RectangleShape2D();
		rect.Size = new Vector2(25, 15);

		AttackHitbox hitbox = AttackHitbox.Create(new AttackHitboxConfig
		{
			ParentNode = this,
			LocalOffset = new Vector2(15, 0),
			HitboxDirection = facingDirection,
			Damage = primaryDamage,
			Duration = primaryDuration,
			Shape = rect,
			KnockbackDirection = facingDirection,
			KnockbackStength = primaryKnockbackAmount,
			AffectsTargets = Targets.EnemiesOnly,
		});

		//keep alive for duration
		await hitbox.Run();

		//attack end delay
		//await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);

		//end attack
		isAttacking = false;
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
		Math.Max(damage, 0);
		//reduce health
		currentHp -= damage;
		//apply knockback
		Velocity += impulse;

		//check for death
		if (currentHp <= 0)
		{
			//player dies and enters some game over state
		}
	}
	
	//Reduces hydration by a specific amount every tick
	public void HydrationTick()
	{
		
	}
}
