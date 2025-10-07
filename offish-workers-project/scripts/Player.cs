using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
	//movement
	private string controlMode = "mouse_and_keyboard";
	[Export] protected int maxSpeed = 600;
	[Export] protected int acceleration = 10;
	[Export] protected int friction = 20;
	// Dodging
	[Export] protected float dodgeForce = 3000;
	[Export] protected float invincibilityCooldown = 0.3f;	// Milliseconds
	[Export] protected float dodgeCooldown = 0.5f;	// Milliseconds
	private bool isDodging = false;
	//stats
	[Export] protected int maxHp = 100;
	protected int currentHp;
	//primary attack
	[Export] protected int primaryDamage = 1;
	[Export] protected float primaryDuration = .2f;
	[Export] protected float primaryKnockbackAmount = 4000;
	
	//Hydration relevant fields
	[Export] protected ProgressBar hydrationBar;
	[Export] protected int hydrationTickLoss = 2; 
	[Export] protected Timer hydrationTimer; 

	private StyleBoxFlat normalHydrationStyle = new StyleBoxFlat(); 
	private StyleBoxFlat lowHydrationStyle = new StyleBoxFlat(); 

	private bool isAttacking = false;
	private Timer chainTimerPrimary = new Timer();
	private int currentChainPrimary = 0;
	private Vector2 movementFacingDirection = Vector2.Right; //default value so player can never face Vector2.zero
	private Vector2 attackFacingDirection = Vector2.Right;

	public override void _Ready()
	{
		base._Ready();
		//add to group for registering attacks
		AddToGroup("player");
		//set collision layer and masks
		
		CollisionLayer = Layers.Bit(Layers.PLAYER);
		CollisionMask = Layers.Bit(Layers.ENVIRONMENT) | Layers.Bit(Layers.ENEMIES) | Layers.Bit(Layers.ENEMY_ATTACKS);
		//CollisionMask = Layers.Bit(Layers.ENVIRONMENT) | Layers.Bit(Layers.ENEMY_ATTACKS);
		
		//Updates currentHP
		currentHp = maxHp;
				
		//Setting up hydration relevant stuff
		hydrationBar = GetNode<ProgressBar>("HydrationBar");
		hydrationBar.MaxValue = maxHp; 
		hydrationBar.Value = currentHp;
		
		normalHydrationStyle.BgColor = new Color("51b5e6"); 
		lowHydrationStyle.BgColor = new Color("e06452"); 
		
		hydrationTimer = GetNode<Timer>("HydrationTimer");
		hydrationTimer.Timeout += OnHydrationTimeout;
		hydrationTimer.Start(); 
	}


	public override void _PhysicsProcess(double delta)
	{
		// Called every frame. Delta is time since the last frame.
		// Update game logic here.

		// Check for attacks
		if (Input.IsActionJustPressed("primary_attack"))
		{
			_ = PrimaryAttack();
		}
		// Check for dodge input
		else if (Input.IsActionJustPressed("dodge_roll"))
		{
			_ = DodgeRoll();
		}
		
		// Movement
		MovePlayer(delta);
		
		// PRESS 0: DEBUG HEALING
		if (Input.IsActionJustPressed("debug_heal"))
		{
			RestoreHydration(30);
			GD.Print("Debug: Healing 30 hp");
		}
		
		// Check for death
		if (currentHp <= 0)
		{
			// Player dies and enters some game over state
			GD.Print("DEAD");
		}
	}

	private void MovePlayer(double delta)
	{
		// Get move input
		Vector2 moveDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		
		// Get attack input (either mouse or right stick)
		// Mouse aiming input will always be detected and overwritten if gamepad input is detected
		// Get the player object's screen position while accounting for camera position
		Vector2 globalPos = GlobalPosition;
		Viewport viewport = GetViewport();
		Camera2D camera = viewport.GetCamera2D();
		Transform2D cameraTransform = camera.GetCanvasTransform();
		Vector2 playerViewportPos = cameraTransform * globalPos;
		
		// Get the mouse position
		Vector2 mousePos = viewport.GetMousePosition();
		
		// Set the player's facing direction
		attackFacingDirection = (mousePos - playerViewportPos).Normalized();
		
		// Overwrite attackFacingDirection with gamepad stick if an input is detected on it
		if (Input.GetVector("face_left", "face_right", "face_up", "face_down") != Vector2.Zero)
		{
			attackFacingDirection = Input.GetVector("face_left", "face_right", "face_up", "face_down");
		}
		
		// TODO: use attackFacingDirection to visualize the attack direction

		// Apply acceleration
		if (moveDirection != Vector2.Zero && !isAttacking)
		{
			Velocity = Velocity.Lerp(moveDirection * maxSpeed, (float)delta * acceleration);
			movementFacingDirection = moveDirection.Normalized();
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
		//dash attack takes priority, then dodge, then regular attack
		//windup, duration, winddown
		//player should still be able to turn during windup phase
		if (isAttacking)
		{
			return;
		}
		isAttacking = true;
		//if timer is stopped, reset chain
		if (chainTimerPrimary.IsStopped())
		{
			currentChainPrimary = 0;
		}
		//else, continue chaining
		else
		{
			currentChainPrimary = (currentChainPrimary + 1) % 3;//3 different attacks in chain
		}
		//start chain timer
		chainTimerPrimary.Start(1f);

		//choose correct attack here

		//to have attacking move the player, add an impulse here

		RectangleShape2D rect = new RectangleShape2D();
		rect.Size = new Vector2(25, 15);

		AttackHitbox hitbox = AttackHitbox.Create(new AttackHitboxConfig
		{
			ParentNode = this,
			LocalOffset = new Vector2(15, 0),
			HitboxDirection = attackFacingDirection,
			Damage = primaryDamage,
			Duration = primaryDuration,
			Shape = rect,
			KnockbackDirection = attackFacingDirection,
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
	
	private async Task DodgeRoll()
	{
		if (isDodging)
		{
			return;
		}
		isDodging = true;
		
		// Player becomes invincible and is pushed forwards
		CollisionMask = Layers.Bit(Layers.ENVIRONMENT);
		Velocity += movementFacingDirection * dodgeForce;
		GD.Print("Invincible");
		
		// Wait for invincibility cooldown
		await ToSignal(GetTree().CreateTimer(invincibilityCooldown), SceneTreeTimer.SignalName.Timeout);
		
		// Player is no longer invincible
		CollisionMask = Layers.Bit(Layers.ENVIRONMENT) | Layers.Bit(Layers.ENEMIES) | Layers.Bit(Layers.ENEMY_ATTACKS);
		//CollisionMask = Layers.Bit(Layers.ENVIRONMENT) | Layers.Bit(Layers.ENEMY_ATTACKS);
		GD.Print("Not invincible");
		
		// Player is able to dodge again
		await ToSignal(GetTree().CreateTimer(dodgeCooldown), SceneTreeTimer.SignalName.Timeout);
		
		isDodging = false;
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
		
		
	}
	
	//Reduces hydration by a specific amount every tick
	private void OnHydrationTimeout()
	{
		currentHp -= hydrationTickLoss; 
		hydrationBar.Value = currentHp; 
		GD.Print("Lost hydration!");
		
		if (((float)currentHp / (float)maxHp) <= 0.25){
			hydrationBar.AddThemeStyleboxOverride("fill", lowHydrationStyle);
		}
		else {
			hydrationBar.AddThemeStyleboxOverride("fill", normalHydrationStyle);
		}
	}
	
	//Restores hydration by a specified amount, usually called when
	//interacting with breakable hydration object
	private void RestoreHydration(int amount)
	{
		currentHp += amount;
		
		if (currentHp > maxHp)
		{
			currentHp = maxHp; 
		}
		
		hydrationBar.Value = currentHp; 
	}
	
	private void DamageHydration(int amount)
	{
		currentHp -= amount; 
		
		if (currentHp <= 0){
			currentHp = 0; 
			GD.Print("DEAD");
		}
		
		hydrationBar.Value = currentHp; 
	}
}
