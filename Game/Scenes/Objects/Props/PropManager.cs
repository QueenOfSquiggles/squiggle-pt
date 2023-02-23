using Godot;
using queen.extension;
using System;

public partial class PropManager : Node
{

    [Export] private int active_stage = 0;
    [Export] private Material active_material;

    [ExportGroup("Paths")]
    [Export] private NodePath path_mesh_instance;
    [Export] private NodePath path_interaction_trigger;

    private MeshInstance3D mesh;
    private InteractiveTrigger trigger;

    public override void _Ready()
    {
        this.GetNode(path_mesh_instance, out mesh);
        this.GetNode(path_interaction_trigger, out trigger);
        GameStages.StageChanged += OnStageChanged;
        trigger.OnInteracted += HandleInteraction;
        OnStageChanged(GameStages.Current);
    }

    private void OnStageChanged(int stage)
    {
        if (stage == active_stage)
        {
            mesh.MaterialOverlay = active_material;
            trigger.is_active = true;
        } else {
            mesh.MaterialOverlay = null;
            trigger.is_active = false;
        }
    }

    private void HandleInteraction()
    {
        GameStages.TriggerNextStage();
    }

    public override void _ExitTree()
    {
        GameStages.StageChanged -= OnStageChanged;
    }
}
