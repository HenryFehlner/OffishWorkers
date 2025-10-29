using Godot;
using System;
using System.Threading.Tasks;

public partial class BomberEnemy : Enemy
{
	public override void _Ready()
	{
		base._Ready();
		maxHp = 5;
		//Long attack cooldown to act like a fuse
		attackCooldown = 5.0f;
		detectionRadius = 350;
		//Start the attack as soon as it detects the player
		attackRadius = detectionRadius;
		enemyType = "printerBomber";
	}

	public override void _PhysicsProcess(double delta)
	{
		// attack
		if (GlobalPosition.DistanceTo(_player.GlobalPosition) < attackRadius)
		{
			// only start the timer when entering attack radius for the first time
			if(!isInAttackRadius)
			{
				attackCooldownTimer.Start(attackCooldown);
				// Prevent the player form just leaving the detection radius
				detectionRadius = 1000;
				attackRadius = 1000;
				isInAttackRadius = true;
			}
			
			_ = Attack();
			
			// Start flash charge
			FlashCharge(attackCooldown);
		}
		else
		{
			isInAttackRadius = false;
			
			// Cancel flash charge
			enemyShaderMat.SetShaderParameter("is_white", false);
		}

		// movement
		Move(delta);
		UpdateHealthBar();
	}

	protected override async Task Attack()
	{
		if (attackCooldownTimer.TimeLeft > 0f)
		{
			return;
		}
		isAttacking = true;

		Shape2D hitboxShape;
		AttackHitboxConfig attackConfig;

		Vector2 attackFacingDirection = (_player.GlobalPosition - this.GlobalPosition).Normalized();

		//shape
		hitboxShape = new RectangleShape2D
		{
			Size = new Vector2(250/Scale.X,250/Scale.Y)
		};
		// attack config
		// TO FIX: Attack is doing damage multiple times to the player
		// Tries 11 hits on the player
		attackConfig = new AttackHitboxConfig
		{
			Owner = this,
			ParentNode = this,
			LocalOffset = new Vector2(1,0),
			HitboxDirection = attackFacingDirection,
			Damage = 3,
			Duration = .2f,
			Shape = hitboxShape,
			KnockbackDirection = attackFacingDirection,
			KnockbackStength = 25,
			AffectsTargets = Targets.PlayerOnly,
		};

		AttackHitbox hitbox = AttackHitbox.Create(attackConfig);

		await hitbox.Run();

		// end attack
		isAttacking = false;
		attackCooldownTimer.Start(attackCooldown);

		// Destroy the enemy after the attack is finished
		currentHp = 0;
		isDead = true;
		QueueFree();

	}
}
