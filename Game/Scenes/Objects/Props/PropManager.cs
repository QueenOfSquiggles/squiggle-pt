using System;
using Godot;
using queen.extension;

public partial class PropManager : Node
{

    public enum PropType
    {
        STAGE_PROGRESSION, MEMORY
    }

    [Export] private int active_stage = 0;
    [Export] private PropType prop_type = PropType.STAGE_PROGRESSION;
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
        if (prop_type == PropType.STAGE_PROGRESSION) GameStages.TriggerNextStage();
        else {
            trigger.is_active = false;
            GameStages.FoundMemory();
        }
    }

    public override void _ExitTree()
    {
        GameStages.StageChanged -= OnStageChanged;
    }
}
