namespace menus;

using Godot;
using queen.extension;
using System;

public partial class MainMenu : Control
{
    [Export(PropertyHint.File, "*.tscn")] private string play_scene;
    [Export(PropertyHint.File, "*.tscn")] private string options_scene;
    [Export(PropertyHint.File, "*.tscn")] private string credits_scene;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }
    private void OnBtnPlay() => Scenes.LoadSceneAsync(play_scene);

    private void OnBtnOptions() => Scenes.LoadSceneAsync(options_scene);

    private void OnBtnCredits() => Scenes.LoadSceneAsync(credits_scene);

    private void OnBtnQuit() => GetTree().Quit();


}
