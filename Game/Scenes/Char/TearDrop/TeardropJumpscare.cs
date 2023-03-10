using System;
using Godot;

public partial class TeardropJumpscare : Node3D
{

	[Export(PropertyHint.File, "*.tscn")] private string GameOverScene;

	public override void _Ready()
	{
        // clear brain
		GetTree().Paused = true;
    }

	public void ExitJumpscare()
	{
		GetTree().Paused = false;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		Scenes.LoadSceneAsync(GameOverScene);
	}
}
