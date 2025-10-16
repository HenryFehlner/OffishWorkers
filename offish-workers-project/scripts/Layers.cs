using Godot;
using System;

public static class Layers
{
    public const int ENVIRONMENT = 0;
    public const int PLAYER = 1;
    public const int ENEMIES = 2;
    public const int PLAYER_ATTACKS = 3;
    public const int ENEMY_ATTACKS = 4;
    public const int DODGE = 5;

    public static uint Bit(int layerIndex) => 1u << layerIndex;
}
