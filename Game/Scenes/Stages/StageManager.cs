using Godot;
using queen.error;
using queen.extension;
using System;

public partial class StageManager : Node
{

    [Export] private string[] stage_names;
    [Export] private NodePath path_hud_requests;
    private HUDRequests hud;

    public override void _Ready()
    {
        this.GetNode(path_hud_requests, out hud);
        GameStages.StageChanged += OnStageChanged;
        OnStageChanged(GameStages.Current);
    }

    public override void _ExitTree()
    {
        GameStages.StageChanged -= OnStageChanged;
    }

    private void OnStageChanged(int stage)
    {
        if (stage >= 0 && stage < stage_names.Length)
        {
            hud.RequestAlert($"Find the '{stage_names[stage]}'");
        } else {
            hud.RequestAlert("Sequence complete");
        }
    }

}
