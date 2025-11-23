using Godot;
using System;

public partial class LevelLoader : Area2D
{
    [Export] protected PackedScene targetScene;

    public override void _Ready()
    {
        GD.Print($"LevelLoader _Ready on node: {Name}");
        BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
		Monitoring = true;
        CollisionLayer = Layers.Bit(Layers.ENEMIES);
        CollisionMask = Layers.Bit(Layers.PLAYER) | Layers.Bit(Layers.DODGE);
        GD.Print("end of ready");
    }

    private void OnBodyEntered(Node2D body) => NextScene(body);
	private void OnAreaEntered(Area2D area) => NextScene(area);

    private void NextScene(Node target)
    {
        GD.Print("In NextScene: player=="+target.IsInGroup("player"));
        if(target.IsInGroup("player") && targetScene!=null)
        {
            GetTree().ChangeSceneToPacked(targetScene);
        }
    }
}
