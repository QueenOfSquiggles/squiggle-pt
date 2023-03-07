using Godot;
using System;

public partial class DemoSplit : Node
{


    [Export] private bool test = false;
    [Export] private int final_stage = 4;
    [Export(PropertyHint.File, "*.tscn")] private string path_demo_end_scene;

    public override void _Ready()
    {
        #if !DEBUG
            test = false;
        #endif
        GameStages.StageChanged += OnStageChange;
    }

    public override void _ExitTree()
    {
        GameStages.StageChanged -= OnStageChange;
    }

    private void OnStageChange(int stage)
    {
        if (stage <= final_stage) return;
        
        if (OS.HasFeature("demo"))
            Scenes.LoadSceneAsync(path_demo_end_scene);            
        else if (test) 
            Scenes.LoadSceneAsync(path_demo_end_scene);
        
    }


}
