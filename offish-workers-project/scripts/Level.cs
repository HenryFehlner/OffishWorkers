using Godot;
using System;
using System.Collections.Generic; 

public partial class Level : Node2D
{
	//PURPOSE OF LEVEL
	//storage for essential level information	

	//NEEDS
	/*
	List of enemies
	Tilemaps with completed level layout
	breakable items
	Entry and exit positions
	Hydration items
	*/
	
	public List<Enemy> enemiesList; 
	
	public override void _Ready()
	{
		
	}

}
