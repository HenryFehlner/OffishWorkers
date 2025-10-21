using Godot;
using System;
using System.Threading.Tasks;

public partial class Enemy : CharacterBody2D
{	
	[Export] protected int maxSpeed = 600;
	[Export] protected int acceleration = 10;
	[Export] protected int friction = 20;
	[Export] protected int maxHp = 15;
	[Export] protected float knockbackMultiplier = 1.0f;
	[Export] protected int detectionRadius = 500;
	protected int currentHp;
	private CharacterBody2D _player;
	[Export] private bool isDead = false;
	
	[Export] public Vector2 spawnPosition;
	[Export] private string enemyType;

	private bool isAttacking = false;
	[Export] protected int attackRadius = 250;
	protected bool isInAttackRadius = false;
	[Export] protected float attackCooldown = 1.5f;
	protected Timer attackCooldownTimer;
	
	private StyleBoxFlat healthStyle = new StyleBoxFlat(); 
	
	private ProgressBar healthBar; 

	public string EnemyType
	{
		get { return enemyType; }
	}

	public bool IsDead
	{
		get { return isDead; }
	}
	
	public void SetPlayer(CharacterBody2D player)
	{
		_player = player;
	}

	public override void _Ready()
	{
		base._Ready();
		
		//add to group for registering attacks
		AddToGroup("enemies");
		//set collision layer and masks
		CollisionLayer = Layers.Bit(Layers.ENEMIES);
		CollisionMask = Layers.Bit(Layers.ENVIRONMENT) | Layers.Bit(Layers.PLAYER_ATTACKS) | Layers.Bit(Layers.PLAYER) | Layers.Bit(Layers.ENEMIES);
		//init attack timer
		attackCooldownTimer = new Timer
		{
			OneShot = true,
			Autostart = false,
			ProcessMode = ProcessModeEnum.Inherit
		};
		AddChild(attackCooldownTimer);

		if (enemyType == null)
		{
			enemyType = "punchingBag";
		}
		
		healthBar = GetNode<ProgressBar>("HealthBar");
		healthBar.MaxValue = maxHp; 
		healthBar.Value = currentHp;
		
		healthStyle.BgColor = new Color("e06452"); 
		healthBar.AddThemeStyleboxOverride("fill", healthStyle);
			
		spawnPosition = this.Position; 
		currentHp = maxHp;
		isDead = false;

	}

	public override void _PhysicsProcess(double delta)
	{
		//attack
		if (GlobalPosition.DistanceTo(_player.GlobalPosition) < attackRadius)
		{
			//reset attack timer if entering attack radius for the first time
			if(!isInAttackRadius)
			{
				attackCooldownTimer.Start(attackCooldown);
				isInAttackRadius = true;
			}
			_ = Attack();
		}
		else
		{
			isInAttackRadius = false;
		}

		//movement
		Move(delta);
		UpdateHealthBar(); 
	}

	protected virtual void Move(double delta)
	{
		if(_player == null)
		{
			 return;
		}
		// Get the direction from enemy to player
		Vector2 direction = (_player.GlobalPosition - this.GlobalPosition).Normalized();
		// Only move towards the player if the player is within the detection radius
		if(GlobalPosition.DistanceTo(_player.GlobalPosition) < detectionRadius)
		{
			// Calculate velocity, accounting for acceleration and max speed
			Velocity = Velocity.Lerp(direction * maxSpeed, (float)delta * acceleration);
			// Apply Friction
			Velocity = Velocity.Lerp(Vector2.Zero, friction * (float)delta);
		}
		else
		{
			Velocity = Velocity.Lerp(Vector2.Zero, friction * (float)delta);
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
		//find velocity cap
		float cap = Math.Max((impulse * knockbackMultiplier).Length(), maxSpeed);
		//cap velocity to limit knockback stacking
		Velocity.Clamp(-cap, cap);
		//check for death
		if (currentHp <= 0)
		{
			//delete enemy
			isDead = true; 
			QueueFree();
		}

		//apply interrupt effects (very basic for now, can implement damage thresholds or num hits taken systems later)
		if (attackCooldownTimer.TimeLeft < .5f)
		{
			attackCooldownTimer.Start(.5f);
		}
	}

	private async Task Attack()
	{
		//based on the issues with numbers, there appear to be two different scaling systems going on
		//it feels like enemies are being scaled up, but the hitbox sizes might be relative to the base sprite size?

		//dash attack takes priority, then dodge, then regular attack
		//windup, duration, winddown
		//player should still be able to turn during windup phase
		if (isAttacking || attackCooldownTimer.TimeLeft > 0f)
		{
			return;
		}
		isAttacking = true;
		//restart cooldown timer
		attackCooldownTimer.Start(attackCooldown);
		//else, continue chaining
		
		//choose correct attack here
		Shape2D hitboxShape;
		AttackHitboxConfig attackConfig;

		//find attack direction (this should probably be calculated at the start of the attack, before the windup)
		Vector2 attackFacingDirection = _player.GlobalPosition - this.GlobalPosition;
		
		//apply impulse
		//Velocity += attackFacingDirection * 1000;
		//shape
		hitboxShape = new RectangleShape2D
		{
			Size = new Vector2(150, 100)
		};
		//attack config
		attackConfig = new AttackHitboxConfig
		{
			Owner = this,
			ParentNode = this,
			LocalOffset = new Vector2(100, 0),
			HitboxDirection = attackFacingDirection,
			Damage = 10,
			Duration = .2f,
			Shape = hitboxShape,
			KnockbackDirection = attackFacingDirection,
			KnockbackStength = 25,
			AffectsTargets = Targets.PlayerOnly,
		};

		//to have attacking move the player, add an impulse here
		AttackHitbox hitbox = AttackHitbox.Create(attackConfig);
		

		//keep alive for duration
		await hitbox.Run();

		//attack end delay
		//await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);

		//end attack
		isAttacking = false;
	}
	
	public void UpdateHealthBar()
	{
		if (currentHp <= 0){
			healthBar.Value = 0; 
		}
		
		healthBar.Value = currentHp; 
	}

}
