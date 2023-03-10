using System;
using Godot;

public partial class TeardropLocation : Marker3D
{
    [Export] public bool IsStandingPosition = true;
    [Export] public string AnimKey = "Standing";
    public bool IsActive = true;

	private void OnEnterScreen() => IsActive = false;
	private void OnExitScreen() => IsActive = true;
}
