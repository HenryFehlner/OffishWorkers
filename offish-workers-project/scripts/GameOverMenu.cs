using Godot;
using System;

public partial class GameOverMenu : Node
{
    public override void _Ready()
    {
        GD.Print("GAME OVER _Ready");
		//Connect button signals 
		GetNode<Button>("VBoxContainer/GameOverMainMenuButton").Pressed += OnMainMenuPressed;
		
		//This following section centerizes the mainmenu based on the viewpoint size
		//Gets the viewport size
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		
		//Get reference to VBoxContainer
		var vbox = GetNode<Control>("VBoxContainer");
		
		//Size of the VBox
		Vector2 vboxSize = vbox.Size;
		
		//Center position
		Vector2 centeredPos = (screenSize - vboxSize) / 2f;
		
		//set it to center
		vbox.Position = centeredPos;
		
		//Add background
		// var bg = GetNode<TextureRect>("Background");
		// bg.Texture = GD.Load<Texture2D>("res://assets/sprites/TempBackground.png");
		// bg.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		// bg.StretchMode = TextureRect.StretchModeEnum.Scale;
		// bg.Visible = true;

	}

	private void OnMainMenuPressed()
	{
		GD.Print("Main Menu clicked");
		
		//update game state
		StateManager.CurrentState = GameStateEnums.GameState.MainMenu;
		
		//load menu scene
		GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
		//removes the menu
		QueueFree();
	}
}
