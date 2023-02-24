using Godot;
using System;

public partial class ResetStages : Node
{

    public override void _Ready() => GameStages.Reset();

}
