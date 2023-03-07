using Godot;
using System;

public partial class TeardropLocation : Marker3D
{
	public bool IsActive = true;

	private void OnEnterScreen() => IsActive = false;
	private void OnExitScreen() => IsActive = true;
}
