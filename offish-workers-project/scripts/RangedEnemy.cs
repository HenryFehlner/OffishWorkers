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
		enemyType = "staplerThrower";
	}
}
