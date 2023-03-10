using System;
using Godot;
using queen.events;

public partial class ObjectSelectSFX : AudioStreamPlayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Events.GUI.MarkAbleToInteract += OnCanInteract;
    }

	public override void _ExitTree()
	{
        Events.GUI.MarkAbleToInteract -= OnCanInteract;
	}

    private void OnCanInteract(string _prompt) => Play();

}
