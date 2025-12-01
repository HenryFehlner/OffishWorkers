using Godot;
using System;

public partial class LevelLoader : Area2D
{
    [Export] protected PackedScene targetLevel;
    protected Node levelNode;

    public override void _Ready()
    {
        GD.Print($"LevelLoader _Ready on node: {Name}");
        BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
		Monitoring = true;
        CollisionLayer = Layers.Bit(Layers.ENEMIES);
        CollisionMask = Layers.Bit(Layers.PLAYER) | Layers.Bit(Layers.DODGE);
        levelNode = GetNode<Node>("/root/Node2D/Level Container/Level");
        GD.Print("levelNode: "+levelNode);
        if(levelNode==null)
        {
            GD.Print("its null");
        }
        GD.Print("end of ready");
    }

    private void OnBodyEntered(Node2D body) => NextScene(body);
	private void OnAreaEntered(Area2D area) => NextScene(area);

    private void NextScene(Node target)
    {
        GD.Print("In NextScene: player=="+target.IsInGroup("player"));
        if(target.IsInGroup("player") && targetLevel!=null)
        {
            GD.Print("in if statement");//this isnt printing!!!!!!!!!!!!!!!!!!!!!!!!!!
            //GetTree().ChangeSceneToPacked(targetLevel);
            levelNode = targetLevel.Instantiate();
            levelNode.AddChild(new Level());
            levelNode.QueueFree();
        }
    }
}
