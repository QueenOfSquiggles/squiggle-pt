using Godot;
using queen.data;
using queen.extension;
using System;

public partial class HangingLight : Node3D
{
    /// Forces the light to remain on, regardless of flashing light accessibility settings
    [Export] private bool IsStable = false;
    [Export] private int BreakingStage = 2;

    [ExportGroup("Paths")]
    [Export] private NodePath path_anim;
    private AnimationPlayer anim;

    public override void _Ready()
    {
        this.GetNode(path_anim, out anim);
        GameStages.StageChanged += BreakLights;
    }

    public override void _ExitTree() 
        => GameStages.StageChanged -= BreakLights;
    

    private void BreakLights(int stage)
    {
        if (!Access.Instance.PreventFlashingLights && !IsStable && stage == BreakingStage)
        {
            anim.Play("Flashing");
            var r = new Random();
            anim.Seek(r.NextSingle() * anim.CurrentAnimationLength); // seek to random position to offset the lights
        }
    }
}
