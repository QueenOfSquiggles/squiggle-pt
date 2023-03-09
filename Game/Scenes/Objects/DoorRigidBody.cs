using System;
using Godot;

public partial class DoorRigidBody : StaticBody3D
{
    [Signal] public delegate void OnRequestOpenEventHandler();

    public bool IsOpen = false;

    public void RequestOpen()
	{
        EmitSignal(nameof(OnRequestOpen));
    }

    public void MarkOpen(bool is_open) => IsOpen = is_open;


}
