using Godot;

[GlobalClass]
public partial class CustomerData : Resource
{
    [Export]
    public Texture2D Body { get; set; }

    [Export]
    public Texture2D Face { get; set; }

    [Export]
    public Texture2D Hand { get; set; }
}