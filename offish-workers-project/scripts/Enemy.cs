using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    [Export] protected int maxSpeed = 0;
	[Export] protected int acceleration = 0;
    [Export] protected int friction = 20;
    [Export] protected int maxHp = 100;
    [Export] protected float knockbackMultiplier = 1.0f;
    protected int currentHp;

    public override void _Ready()
    {
        base._Ready();
        //add to group for registering attacks
        AddToGroup("enemies");
        //set collision layer and masks
        CollisionLayer = Layers.Bit(Layers.ENEMIES);
        CollisionMask = Layers.Bit(Layers.ENVIRONMENT) | Layers.Bit(Layers.PLAYER) | Layers.Bit(Layers.PLAYER_ATTACKS);

        currentHp = maxHp;
    }

    public override void _PhysicsProcess(double delta)
    {
        //movement
        Move(delta);
    }

    protected virtual void Move(double delta)
    {
        Velocity = Velocity.Lerp(Vector2.Zero, (float)delta * friction);
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
