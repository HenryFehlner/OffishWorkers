using Godot;
using System;

public partial class LevelLoader : Node2D
{
    [Export] protected PackedScene targetScene;

    public override void _Ready()
    {
        base._Ready();
        Area2D parent = GetParent<Area2D>();
        parent.BodyEntered += OnBodyEntered;
		parent.AreaEntered += OnAreaEntered;
		parent.Monitoring = true;
    }

    private void OnBodyEntered(Node body) => NextScene(body);
	private void OnAreaEntered(Area2D area) => NextScene(area);

    private void NextScene(Node target)
    {
        if(target.IsInGroup("player"))
        {
            GetTree().ChangeSceneToPacked(targetScene);
        }
    }
}
