using Godot;
using System;

public class LevelBackground : ParallaxBackground
{
    private ParallaxLayer parallaxLayer;
    [Export]
    private Vector2 motionSpeed = new Vector2(0, 100);

    public override void _Ready()
    {
        parallaxLayer = GetNode<ParallaxLayer>("ParallaxLayer");

    }

}