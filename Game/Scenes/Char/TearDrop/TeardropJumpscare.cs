using Godot;
using System;

public partial class TeardropJumpscare : CanvasLayer
{

	[Export(PropertyHint.File, "*.tscn")] private string GameOverScene;

	public override void _Ready()
	{
		Scenes.LoadSceneAsync(GameOverScene, true);
	}

	public void ExitJumpscare()
	{
		GetTree().Paused = false;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		QueueFree();
	}
}
