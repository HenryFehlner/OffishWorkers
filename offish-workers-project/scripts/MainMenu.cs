using Godot;
using System;

public partial class MainMenu : Control
{
	public override void _Ready()
	{
		//Connect button signals 
		GetNode<Button>("VBoxContainer/StartButton").Pressed += OnStartPressed;
		GetNode<Button>("VBoxContainer/SettingsButton").Pressed += OnSettingsPressed;
		GetNode<Button>("VBoxContainer/ExitButton").Pressed += OnExitPressed;
		
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
		var bg = GetNode<TextureRect>("Background");
		bg.Texture = GD.Load<Texture2D>("res://assets/sprites/TempBackground.png");
		bg.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		bg.StretchMode = TextureRect.StretchModeEnum.Scale;
		bg.Visible = true;

	}

	private void OnStartPressed()
	{
		GD.Print("Start Game clicked");
		
		//update game state
		StateManager.CurrentState = GameStateEnums.GameState.GamePlay;
		
		//load main scene
		PackedScene gameplayScene = GD.Load<PackedScene>("res://scenes/main.tscn");
		Node instance = gameplayScene.Instantiate();
		GetParent().AddChild(instance);
		GetTree().ChangeSceneToFile("res://scenes/main.tscn");
		//removes the menu
		QueueFree();
	}
	
	private void OnSettingsPressed()
	{
		var popup = GetNode<AcceptDialog>("ControlsPopup");
		//Centers and shows it
		popup.PopupCentered();
	}
	
	private void OnExitPressed()
	{
		GD.Print("Exit clicked — quitting game");
		GetTree().Quit();
	}
}
