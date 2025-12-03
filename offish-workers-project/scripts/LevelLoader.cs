using Godot;
using System;

public partial class LevelLoader : Area2D
{
	protected Node levelNode;
	protected gameplayController gameplayController;

	public override void _Ready()
	{
		GD.Print($"LevelLoader _Ready on node: {Name}");

		//triggers
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
		Monitoring = true;
		//collisions
		CollisionLayer = Layers.Bit(Layers.ENEMIES);
		CollisionMask = Layers.Bit(Layers.PLAYER) | Layers.Bit(Layers.DODGE);

		levelNode = GetNode<Node>("/root/Node2D/Level Container/Level");
		gameplayController = GetNode<gameplayController>("/root/Node2D/Gameplay Controller");
		GD.Print("levelNode: "+levelNode);
		GD.Print("gameplayController: "+gameplayController);

		if(gameplayController==null)
		{
			GD.Print("gameplayController is null");
		}
		GD.Print("end of ready");
	}

	private void OnBodyEntered(Node2D body) => NextScene(body);
	private void OnAreaEntered(Area2D area) => NextScene(area);

	private void NextScene(Node target)
	{
		if(!target.IsInGroup("player")||gameplayController==null) return;

		GD.Print("In NextScene: player=="+target.IsInGroup("player"));
		
		gameplayController.CallDeferred(nameof(gameplayController.LoadNextLevel));
	}
}
