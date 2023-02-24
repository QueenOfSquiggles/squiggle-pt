using Godot;
using System;

public partial class VersioningLabel : Label
{
    [Export] private string text_release = "Full Release";
    [Export] private string text_demo = "Demo Version";

    public override void _Ready()
    {
        #if GODOT_DEMO
            Text = text_demo;
        #else
            Text = text_release;
        #endif

        #if DEBUG
            Text += " - debug";
        #endif
        if (OS.HasFeature("demo"))
        {
            Text += " -- demo";
        } else {
            Text += " -- release";
        }
    }
    

}
