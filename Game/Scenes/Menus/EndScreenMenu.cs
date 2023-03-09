using Godot;
using System;

public partial class EndScreenMenu : Control
{
	[Export(PropertyHint.File, "*.tscn")] private string MainMenuScene; 

	public void LoadMainMenu()
	{
		Scenes.LoadSceneAsync(MainMenuScene);
	}
}
