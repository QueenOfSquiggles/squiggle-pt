using Godot;
using squiggle_zone.modules.data;

public partial class ForceAccessLoad : Node
{

    public override void _Ready()
    {
        var _ = Access.Instance.FontOption; // forcing a load of the instance
    }
}
