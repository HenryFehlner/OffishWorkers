using Godot;
using System;
using System.Threading.Tasks;

public partial class RangedEnemy : Enemy
{
	public override void _Ready()
	{
		base._Ready();
		// Make this enemy stationary
		maxSpeed = 0;
		acceleration = 0;
		friction = 35;
		maxHp = 10;
		// Attack Radius should be the same as the detection radius
		attackRadius = detectionRadius;
		enemyType = "staplerThrower";
	}
	protected override void SpawnDeathDrops()
	{
		//Node hydrationContainer = GetNode<Node>("../../../Hydration Container");
		hydrationContainer.CallDeferred(Node.MethodName.AddChild, hydrationRestoreObject.Create(GlobalPosition));
	}

	protected override async Task Attack()
	{
		if (attackCooldownTimer.TimeLeft > 0)
		{
			return;
		}
		attackCooldownTimer.Start(attackCooldown);
		//Firing the projectile doesn't really need to lock the player down, so no need for async
		Shape2D hitboxShape = new RectangleShape2D
				{
					Size = new Vector2(50/Scale.X, 50/Scale.Y)
				};
		Projectile proj = Projectile.Create(new ProjectileConfig
		{
			Owner = this,
			ParentNode = this,
			HitboxDirection = (_player.GlobalPosition - this.GlobalPosition).Normalized(),
			Damage = 1,
			Duration = 5,
			Shape = hitboxShape,
			KnockbackDirection =  (_player.GlobalPosition - this.GlobalPosition).Normalized(),
			KnockbackStength = 500,
			AffectsTargets = Targets.PlayerOnly,
			StartPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y),
			Speed = 700,
			MovementDirection =  (_player.GlobalPosition - this.GlobalPosition).Normalized(),
			Pierce = 0,
			stopsOnEnvironment = true
		});
		proj.ZIndex = 1;
		//add projectile to scene
		GetTree().CurrentScene.AddChild(proj);
	}
}
