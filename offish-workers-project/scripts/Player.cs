using Godot;
using System;

public partial class Player : Node
{
	protected int speed = 400;
	protected int accel = 5;
	
	getInput();
	
	public void getInput()
	{
		//vector2 direction = Input.get_vector("move_left", "move_right", "move_up", "move_down");
		//print(direction);
	}
}
