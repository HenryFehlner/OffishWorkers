using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
	//masks
	private uint defaultCollisionLayer = Layers.Bit(Layers.PLAYER);
	private uint defaultCollisionMask = Layers.Bit(Layers.ENVIRONMENT) | Layers.Bit(Layers.ENEMIES) | Layers.Bit(Layers.ENEMY_ATTACKS);

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
	//primary attack (unused for now)
	[Export] protected int primaryDamage = 1;
	[Export] protected float primaryDuration = .2f;
	[Export] protected float primaryKnockbackAmount = 4000;
	
	//Hydration relevant fields
	[Export] protected ProgressBar hydrationBar;
	[Export] protected int hydrationTickLoss = 2; 
	[Export] protected Timer hydrationTimer; 

	//Able to be get and set
	[Export] private bool onHydrationRestore = false;
	
	//Shows the direction indicator
	[Export] public NodePath DirectionIndicatorPath;
	//reference to the indicator
	private Sprite2D directionIndicator;
	
	[Signal] public delegate void PlayerDeathEventHandler();
	
	public bool OnHydrationRestore
	{
		set {
			onHydrationRestore = value; 
		}
	}
	
	private StyleBoxFlat normalHydrationStyle = new StyleBoxFlat(); 
	private StyleBoxFlat lowHydrationStyle = new StyleBoxFlat(); 

	private bool isAttacking = false;
	private Timer chainTimerPrimary;
	private int currentChainPrimary = 0;
	private Vector2 movementFacingDirection = Vector2.Right; //default value so player can never face Vector2.zero
	private Vector2 attackFacingDirection = Vector2.Right;

	//secondary attack stuff
	[Export] private float secondaryCooldown = 1;
	private Timer secondaryCooldownTimer;
	
	// Player sprite and its shader material
	private AnimatedSprite2D playerSprite;
	private ShaderMaterial playerShaderMat;

	public override void _Ready()
	{
		base._Ready();
		//add to group for registering attacks
		AddToGroup("player");
		//set collision layer and masks
		CollisionLayer = defaultCollisionLayer;
		CollisionMask = defaultCollisionMask;
		//setup attack combo timer
		chainTimerPrimary = new Timer
		{
			OneShot = true,
			Autostart = false,
			ProcessMode = ProcessModeEnum.Inherit
		};
		AddChild(chainTimerPrimary);
		//secondary attack cooldown timer
		secondaryCooldownTimer = new Timer
		{
			OneShot = true,
			Autostart = false,
			ProcessMode = ProcessModeEnum.Inherit
		};
		AddChild(secondaryCooldownTimer);
		
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
		
		directionIndicator = GetNode<Sprite2D>("DirectionIndicator");
		
		//Direction indicator setup
/*
		if (DirectionIndicatorPath != null && DirectionIndicatorPath.ToString() != "")
		{
			directionIndicator = GetNode<Sprite2D>(DirectionIndicatorPath);
		}
		else
		{
			GD.PrintErr("DirectionIndicatorPath not set in Inspector.");
		}*/
		
		// Get the player sprite and its material for use with flashing
		playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		playerShaderMat = (ShaderMaterial)playerSprite.Material;
	}


	public override void _PhysicsProcess(double delta)
	{
		// Called every frame. Delta is time since the last frame.
		// Update game logic here.

		// Check for dodge input
		if (Input.IsActionJustPressed("dodge_roll"))
		{
			_ = DodgeRoll();
		}
		// Check for primary attack
		else if (Input.IsActionJustPressed("primary_attack"))
		{
			_ = PrimaryAttack();
		}
		else if (Input.IsActionJustPressed("secondary_attack"))
		{
			SecondaryAttack();
		}
		
		// Movement
		MovePlayer(delta);
		
		//Updates movement based direction indicator
		UpdateDirectionIndicator();

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
			EmitDeathSignal(); 
		}
	}
	
	// Called on every frame with an input
	// Used here to switch between gamepad and MnK controls
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion 
			&& controlMode == "gamepad")
		{
			controlMode = "mouse_and_keyboard";
		}
		else if (Input.GetVector("face_left", "face_right", "face_up", "face_down") != Vector2.Zero 
			&& controlMode == "mouse_and_keyboard")
		{
			controlMode = "gamepad";
		}
	}

	private void MovePlayer(double delta)
	{
		// Get move input
		Vector2 moveDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		movementFacingDirection = moveDirection.Normalized();

		// Get attack input (either mouse or right stick)
		if (controlMode == "mouse_and_keyboard")
		{
			// Get the player object's screen position while accounting for camera position
			Vector2 globalPos = GlobalPosition;
			Viewport viewport = GetViewport();
			Camera2D camera = viewport.GetCamera2D();
			Transform2D cameraTransform = camera.GetCanvasTransform();
			Vector2 playerViewportPos = cameraTransform * globalPos;
			
			// Get the mouse position
			Vector2 mousePos = viewport.GetMousePosition();
			
			// Set the player's facing direction by subtracting the vectors
			attackFacingDirection = (mousePos - playerViewportPos).Normalized();
		}
		else if (controlMode == "gamepad")
		{
			// Cancel input if no stick direction, this saves the last facing direction and prevents it from being Vector2.Zero
			if (Input.GetVector("face_left", "face_right", "face_up", "face_down") == Vector2.Zero)
			{
				goto NoControllerAimInput;
			}
			
			// Get right stick input
			attackFacingDirection = Input.GetVector("face_left", "face_right", "face_up", "face_down").Normalized();
			//savedFacingDirection = attackFacingDirection;
		}
		NoControllerAimInput:
		
		// TODO: use attackFacingDirection to visualize the attack direction (probably put it somewhere else)

		// Apply acceleration
		if (moveDirection != Vector2.Zero && !isAttacking)
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
		
		// Update the sprite look direction
		playerSprite.FlipH = Velocity.X > 0;
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
		if (chainTimerPrimary.TimeLeft <= 0f)
		{
			GD.Print($"Starting new chain");
			currentChainPrimary = 0;
		}
		//else, continue chaining
		else
		{
			currentChainPrimary = (currentChainPrimary + 1) % 3;//3 different attacks in chain
		}
		
		GD.Print($"Attack #{currentChainPrimary}");
		//choose correct attack here
		Shape2D hitboxShape;
		AttackHitboxConfig attackConfig;
		switch (currentChainPrimary)
		{
			default:
				//start chain timer
				chainTimerPrimary.Start(.6f);
				//apply impulse
				Velocity += attackFacingDirection * 1000;
				//shape
				hitboxShape = new RectangleShape2D
				{
					Size = new Vector2(20, 15)
				};
				//attack config
				attackConfig = new AttackHitboxConfig
				{
					Owner = this,
					ParentNode = this,
					LocalOffset = new Vector2(15, 0),
					HitboxDirection = attackFacingDirection,
					Damage = 1,
					Duration = .2f,
					Shape = hitboxShape,
					KnockbackDirection = attackFacingDirection,
					KnockbackStength = 4000,
					AffectsTargets = Targets.EnemiesOnly,
				};
				break;
			case 1:
				//start chain timer
				chainTimerPrimary.Start(.6f);
				//apply impulse
				Velocity += attackFacingDirection * 1000;
				//shape
				hitboxShape = new RectangleShape2D
				{
					Size = new Vector2(25, 15)
				};
				//attack config
				attackConfig = new AttackHitboxConfig
				{
					Owner = this,
					ParentNode = this,
					LocalOffset = new Vector2(15, 0),
					HitboxDirection = attackFacingDirection,
					Damage = 1,
					Duration = .2f,
					Shape = hitboxShape,
					KnockbackDirection = attackFacingDirection,
					KnockbackStength = 4000,
					AffectsTargets = Targets.EnemiesOnly,
				};
				break;
			case 2:
				//start chain timer
				chainTimerPrimary.Start(.6f);
				//apply impulse

				//shape
				hitboxShape = new RectangleShape2D
				{
					Size = new Vector2(25, 25)
				};
				//attack config
				attackConfig = new AttackHitboxConfig
				{
					Owner = this,
					ParentNode = this,
					LocalOffset = new Vector2(15, 0),
					HitboxDirection = attackFacingDirection,
					Damage = 1,
					Duration = .4f,
					Shape = hitboxShape,
					KnockbackDirection = attackFacingDirection,
					KnockbackStength = 7000,
					AffectsTargets = Targets.EnemiesOnly,
				};
				break;
		}

		//to have attacking move the player, add an impulse here
		AttackHitbox hitbox = AttackHitbox.Create(attackConfig);
		
		
		

		//keep alive for duration
		await hitbox.Run();

		//attack end delay
		//await ToSignal(GetTree().CreateTimer(0.8f), SceneTreeTimer.SignalName.Timeout);

		//end attack
		isAttacking = false;
	}


	private void SecondaryAttack()
	{
		if (secondaryCooldownTimer.TimeLeft > 0)
		{
			return;
		}
		secondaryCooldownTimer.Start(secondaryCooldown);
		//Firing the projectile doesn't really need to lock the player down, so no need for async
		Shape2D hitboxShape = new RectangleShape2D
				{
					Size = new Vector2(50, 50)
				};
		Projectile proj = Projectile.Create(new ProjectileConfig
		{
			Owner = this,
			ParentNode = this,
			HitboxDirection = attackFacingDirection,
			Damage = 1,
			Duration = 5,
			Shape = hitboxShape,
			KnockbackDirection = attackFacingDirection,
			KnockbackStength = 500,
			AffectsTargets = Targets.EnemiesOnly,
			StartPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y),
			Speed = 1000,
			MovementDirection = attackFacingDirection,
			Pierce = 0,
			stopsOnEnvironment = true
		});
		//add projectile to scene
		GetTree().CurrentScene.AddChild(proj);
	}
	private async Task DodgeRoll()
	{
		if (isDodging)
		{
			return;
		}
		isDodging = true;
		//dodging cancels attacks
		isAttacking = false;

		// Player becomes invincible and is pushed forwards
		CollisionMask = Layers.Bit(Layers.ENVIRONMENT);
		CollisionLayer = Layers.Bit(Layers.DODGE);
		Velocity += movementFacingDirection * dodgeForce;
		playerShaderMat.SetShaderParameter("is_white", true);
		//GD.Print("Invincible");

		// Wait for invincibility cooldown
		await ToSignal(GetTree().CreateTimer(invincibilityCooldown), SceneTreeTimer.SignalName.Timeout);

		// Player is no longer invincible
		CollisionMask = defaultCollisionMask;
		CollisionLayer = defaultCollisionLayer;
		playerShaderMat.SetShaderParameter("is_white", false);
		//GD.Print("Not invincible");

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
		//find velocity cap
		float cap = Math.Max(impulse.Length(), maxSpeed);
		//cap velocity to limit knockback stacking
		Velocity.Clamp(-cap, cap);
		
	}
	
	//Reduces hydration by a specific amount every tick
	private void OnHydrationTimeout()
	{
		if (onHydrationRestore == false)
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
	}
	
	//Restores hydration by a specified amount, usually called when
	//interacting with breakable hydration object
	public void RestoreHydration(int amount)
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
	
	private void EmitDeathSignal()
	{
		EmitSignal(SignalName.PlayerDeath);
	}
	
	private void UpdateDirectionIndicator()
	{
		if (directionIndicator == null){
			return;
		}

		//Use movement direction for indicator
		Vector2 facing = movementFacingDirection;

		if (!facing.IsZeroApprox())
		{
			directionIndicator.Rotation = facing.Angle();
			//Adjust if needed
			float offset = 12f;
			directionIndicator.Position = facing * offset;
		}
	}
}
