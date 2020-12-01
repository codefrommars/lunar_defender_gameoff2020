using Godot;

public class BaseStage : Node2D
{
	public Position2D PlayerStart { get; private set; }
	private Node2D portalsNode;
	private TileMap tileMap;

	public override void _Ready()
	{
		PlayerStart = GetNode<Position2D>("PlayerStart");
		tileMap = GetNode<TileMap>("TileMap");
		portalsNode = GetNode<Node2D>("Portals");
	}

	public Rect2 GetExtentsRect()
	{
		Rect2 used = tileMap.GetUsedRect();
		used.Position *= MoonHunter.Constants.TILE_SIZE;
		used.Position += GlobalPosition;
		used.Size *= MoonHunter.Constants.TILE_SIZE;
		return used;
	}

	public StagePortal GetPortal(string name)
	{
		return portalsNode.GetNode<StagePortal>(name);
	}

}
