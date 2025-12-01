using Godot;
using System;

public partial class LevelLoader : Area2D
{
    [Export] protected PackedScene targetLevel;
    protected Node levelNode;
    protected gameplayController gameController;

    public override void _Ready()
    {
        GD.Print($"LevelLoader _Ready on node: {Name}");
        BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;
		Monitoring = true;
        CollisionLayer = Layers.Bit(Layers.ENEMIES);
        CollisionMask = Layers.Bit(Layers.PLAYER) | Layers.Bit(Layers.DODGE);
        levelNode = GetNode<Node>("/root/Node2D/Level Container/Level");
        gameController = GetNode<gameplayController>("/root/Node2D/Gameplay Controller");
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
            gameController.LoadNextLevel();
            return;
            GD.Print("in if statement");
            //GetTree().ChangeSceneToPacked(targetLevel);
            //levelNode = targetLevel.Instantiate();
            //levelNode.AddChild(new Level());
            //levelNode.QueueFree();
            //GD.Print("cleared");

            if(levelNode != null && levelNode.IsInsideTree())
            {
                //free levelNode
                levelNode.QueueFree();
            }

            Node2D newLevel = targetLevel.Instantiate<Node2D>();
            newLevel.AddChild(new Level());
            levelNode = newLevel;
            GD.Print("Loaded new level");
            
        }
    }
}
