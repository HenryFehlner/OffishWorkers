using Godot;
using System;
using System.Collections.Generic;

public partial class Projectile : Node2D
{
    //set from projectile config
    protected float speed;
    protected Vector2 movementDirection;
    protected int pierce;//0 stops at first hit, 1 pierces through the first target and is removed on hitting the second, etc.
    protected bool stopsOnEnvironment;
    //set from the hitbox config
    protected Node owner;
    //other fields
    protected AttackHitbox hitbox;
    protected float timeRemaining;

    //creation method
    public static Projectile Create(ProjectileConfig config)
    {
        Projectile proj = new Projectile();
        proj.Configure(config);
        return proj;
    }
    public void Configure(ProjectileConfig config)
    {
        timeRemaining = config.Duration;

        speed = config.Speed;
        movementDirection = config.MovementDirection;
        pierce = config.Pierce;
        stopsOnEnvironment = config.stopsOnEnvironment;

        hitbox = AttackHitbox.Create(new AttackHitboxConfig
        {
            Owner = config.Owner,
            ParentNode = this,
            LocalOffset = Vector2.Zero,
            HitboxDirection = config.HitboxDirection,
            Damage = config.Damage,
            Duration = -1,//use projectile despawning system
            Shape = config.Shape,
            KnockbackDirection = movementDirection,
            KnockbackStength = config.KnockbackStength,
            AffectsTargets = config.AffectsTargets,
        });
        //AddChild(hitbox);

        //subscribe to hitbox handler
        hitbox.Hit += OnHit;

        GlobalPosition = config.StartPosition;
        Rotation = config.HitboxDirection.Angle();
    }

    public override void _PhysicsProcess(double delta)
    {
        float deltaTime = (float)delta;

        //lifetime
        timeRemaining -= deltaTime;
        //if timer runs out, delete the projectile
        if (timeRemaining <= 0f)
        {
            QueueFree();
            return;
        }

        //move the projectile
        Vector2 start = GlobalPosition;
        Vector2 end = start + movementDirection * speed * deltaTime;
        //make sure the projectile does not tunnel through walls
        if (stopsOnEnvironment)
        {
            PhysicsDirectSpaceState2D space = GetWorld2D().DirectSpaceState;
            PhysicsRayQueryParameters2D ray = PhysicsRayQueryParameters2D.Create(start, end);
            ray.CollisionMask = Layers.Bit(Layers.ENVIRONMENT);
            var hit = space.IntersectRay(ray);
            //if it hits the enviornment, despawn it
            if (hit.Count > 0)
            {
                //snap 
                GlobalPosition = (Vector2)hit["position"];
                QueueFree();
                return;
            }
        }
        //move to the position if no collisions with the enviroment detected
        GlobalPosition = end;
        GD.Print(GlobalPosition);
    }

    private void OnHit(Node target)
    {
        GD.Print("In OnHit");
        //check if projectile can still pierce
        if (pierce > 0)
        {
            pierce--;
        }
        else//no more pierces, remove the projectile
        {
            QueueFree();
        }
    }

    public override void _ExitTree()
    {
        //in case hitbox is still somehow alive, destroy it here
        if(IsInstanceValid(hitbox))
        {
            hitbox.QueueFree();
        }
    }

}
public struct ProjectileConfig
{
    //ATTACK HITBOX FIELDS:
    //owner
    public Node Owner;
	//parent node
	public Node ParentNode;
	//the direction the hitbox is facing
	public Vector2 HitboxDirection;
	//how much damage is inflicted
	public int Damage;
	//how many seconds the projectile persists (negative to avoid despawns)
	public float Duration;
	//shape of the hitbox
	public Shape2D Shape;
	//direction of knockback
	public Vector2 KnockbackDirection;
	//strength of knockback
	public float KnockbackStength;
    //what kind of entities can be hit
    public Targets AffectsTargets;
    //PROJECTILE SPECIFIC STARTS HERE:
    //speed of the projectile
    public Vector2 StartPosition;
    public float Speed;
    //the movement direction of the projectile
    public Vector2 MovementDirection;
    //0 stops at first hit, 1 pierces through the first target and is removed on hitting the second, etc.
    public int Pierce;
    //if true, the projectile disappears on colliding with the environment
    public bool stopsOnEnvironment;
}
