using System;
using Godot;
using queen.error;
using queen.extension;

public partial class StageManager : Node
{

    [Export] private string[] stage_names;
    [Export] private NodePath path_hud_requests;
    [Export] private int required_memories = 3;
    [Export(PropertyHint.File, "*.tscn")] private string win_normal_scene;
    [Export(PropertyHint.File, "*.tscn")] private string win_true_scene;
    private HUDRequests hud;

    public override void _Ready()
    {
        this.GetNode(path_hud_requests, out hud);
        GameStages.StageChanged += OnStageChanged;
        GameStages.MemoryFound += OnMemoryFound;
        OnStageChanged(GameStages.Current);
    }

    public override void _ExitTree()
    {
        GameStages.StageChanged -= OnStageChanged;
        GameStages.MemoryFound -= OnMemoryFound;
    }

    private void OnMemoryFound(int memories)
    {
        // TODO play an audio jingle
    }

    private void OnStageChanged(int stage)
    {
        if (stage >= 0 && stage < stage_names.Length)
        {
            hud.RequestAlert($"Find the '{stage_names[stage]}'");
        } else if (stage >= stage_names.Length) {
            hud.RequestAlert("");
            Input.MouseMode = Input.MouseModeEnum.Visible;
            if (GameStages.Memories >= required_memories)
                Scenes.LoadSceneAsync(win_true_scene);
            else
                Scenes.LoadSceneAsync(win_normal_scene);
        }
    }

}
