using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum Targets { EnemiesOnly, PlayerOnly, AllButParent, All }

public partial class AttackHitbox : Area2D
{
	//event handler to allow projectiles to register their hits
	[Signal] public delegate void HitEventHandler(Node target);

	//config stats
	//owner node
	protected Node owner;
	//parent node
	protected Node parentNode;
	//where to place relative to parent node
	protected int damage;
	//how long the hitbox is active for
	protected float duration;
	//what kind of entities can be hit
	protected Targets affectsTargets;

	//other fields
	//collection of nodes already hit by this hitbox
	protected HashSet<Node> hitNodes = new HashSet<Node>();
	//knockback impulse
	protected Vector2 knockbackImpulse;

	/// <summary>
	/// Creation method
	/// </summary>
	/// <param name="parent">Node the attack originates from</param>
	/// <param name="config">Attack config data</param>
	/// <returns>AttackHitbox for the attack</returns>
	public static AttackHitbox Create(AttackHitboxConfig config)
	{
		AttackHitbox hitbox = new AttackHitbox();
		hitbox.Configure(config);
		return hitbox;
	}
	/// <summary>
	/// Configure the hitbox based on config info passed in
	/// </summary>
	/// <param name="config">Configuration struct</param>
	public void Configure(AttackHitboxConfig config)
	{
		//set field data
		owner = config.Owner;
		parentNode = config.ParentNode;
		parentNode.AddChild(this);
		damage = config.Damage;
		duration = config.Duration;
		affectsTargets = config.AffectsTargets;

		//calculate knockback impulse
		knockbackImpulse = config.KnockbackDirection * config.KnockbackStength;

		//visible rectangle for attack feedback during playtest (this will only work correctly with rectangles)
		ColorRect visRect = new ColorRect
		{
			Color = new Color(1, 1, 1, .3f),
			Size = config.Shape.GetRect().Size,
			Position = config.Shape.GetRect().Size/-2f
		};
		AddChild(visRect);

		//add collision shape
		AddChild(new CollisionShape2D { Shape = (Shape2D)config.Shape.Duplicate(true) });
		//layer
		if (owner.IsInGroup("player"))
		{
			CollisionLayer = Layers.Bit(Layers.PLAYER_ATTACKS);
		}
		else
		{
			CollisionLayer = Layers.Bit(Layers.ENEMY_ATTACKS);
		}

		//masks
		CollisionMask = affectsTargets switch
		{
			Targets.PlayerOnly => Layers.Bit(Layers.PLAYER),
			Targets.EnemiesOnly => Layers.Bit(Layers.ENEMIES),
			_ => Layers.Bit(Layers.PLAYER) | Layers.Bit(Layers.ENEMIES),
		};

		//position and rotation
		Rotation = config.HitboxDirection.Angle();
		Position = config.LocalOffset.Rotated(Rotation);


		//signals
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
		Monitoring = true;
	}

	private void OnBodyEntered(Node body) => TryHit(body);
	private void OnAreaEntered(Area2D area) => TryHit(area);

	/// <summary>
	/// Attempt to apply a hit to an object
	/// </summary>
	/// <param name="target">target of the attack</param>
	private void TryHit(Node target)
	{
		GD.Print($"Trying hit on {target}");
		//return if target is null, OR target is the parent and the parent should not be able to be hit by the attack, OR this node has already been hit
		if (target == null || (target == owner && affectsTargets != Targets.All) || hitNodes.Contains(target))
		{
			return;
		}

		bool targetIsPlayer = target.IsInGroup("player");//NEED TO SET UP GROUPS!!!!!!!!!!!!!!!!!!
		bool targetIsEnemy = target.IsInGroup("enemies");

		//check to see if target is able to be hit by this attack
		if ((targetIsPlayer && affectsTargets == Targets.EnemiesOnly) || (targetIsEnemy && affectsTargets == Targets.PlayerOnly))
		{
			return;
		}

		//target is hit by attack
		if (target.HasMethod("TakeHit"))
		{
			//call TakeHit method
			target.Call("TakeHit", damage, knockbackImpulse, owner);
			EmitSignal(SignalName.Hit, target);
		}

	}

	public async Task Run()
	{
		//timer override for projectiles/independent despawning timers
		if(duration<=0)
        {
			return;
        }
		//timer
		SceneTreeTimer timer = GetTree().CreateTimer(duration);
		await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);
		QueueFree();
	}

}

public struct AttackHitboxConfig
{
	//owner (will not hit self with own attack)
	public Node Owner;
	//parent node
	public Node ParentNode;
	//where to place relative to parent node
	public Vector2 LocalOffset;
	//the direction the hitbox is facing
	public Vector2 HitboxDirection;
	//how much damage is inflicted
	public int Damage;
	//how long the hitbox is active for
	public float Duration;
	//shape of the hitbox
	public Shape2D Shape;
	//direction of knockback
	public Vector2 KnockbackDirection;
	//strength of knockback
	public float KnockbackStength;
	//what kind of entities can be hit
	public Targets AffectsTargets;
}
