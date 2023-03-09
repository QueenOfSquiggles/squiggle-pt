using System;
using Godot;

public partial class StageDebugger : Node
{

#if DEBUG

    public override void _Input(InputEvent e)
    {
        if (e is InputEventKey key && key.IsPressed())
        {
            if (key.Keycode == Key.KpAdd) GameStages.TriggerNextStage();
            if (key.Keycode == Key.KpSubtract) GameStages.TriggerPreviousStage();
            if (key.Keycode == Key.KpDivide) GameStages.FoundMemory();
        }
    }

#endif
}
