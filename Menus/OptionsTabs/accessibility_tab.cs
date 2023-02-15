using Godot;
using queen.data;
using queen.error;
using queen.extension;
using System;

public partial class accessibility_tab : PanelContainer
{
    [Export] private NodePath path_checkbox_no_flashing_lights;
    [Export] private NodePath path_slider_rumble_strength;
    [Export] private NodePath path_slider_screen_shake_strength;
    [Export] private NodePath path_slider_rumble_duration;
    [Export] private NodePath path_slider_screen_shake_duration;
    [Export] private NodePath path_slider_max_volume;
    [Export] private NodePath path_slider_engine_time_scale;
    [Export] private NodePath path_option_font;

    private CheckBox checkbox_no_flashing_lights;
    private HSlider slider_rumble_strength;
    private HSlider slider_screen_shake_strength;
    private HSlider slider_rumble_duration;
    private HSlider slider_screen_shake_duration;
    private HSlider slider_max_volume;
    private HSlider slider_time_scale;
    private OptionButton option_font;

    private bool FontHasChanged = false;

    public override void _Ready()
    {
        this.GetNode(path_checkbox_no_flashing_lights, out checkbox_no_flashing_lights);
        this.GetNode(path_option_font, out option_font);
        // The Slider Combo needs to be 'cracked' to access the actual slider node. Not preferable...
        // TODO maybe find a better way to access this node? 
        slider_rumble_strength = GetNode<Control>(path_slider_rumble_strength).GetNode("HSlider") as HSlider;
        slider_screen_shake_strength = GetNode<Control>(path_slider_screen_shake_strength).GetNode("HSlider") as HSlider;
        slider_rumble_duration = GetNode<Control>(path_slider_rumble_duration).GetNode("HSlider") as HSlider;
        slider_screen_shake_duration = GetNode<Control>(path_slider_screen_shake_duration).GetNode("HSlider") as HSlider;
        slider_max_volume = GetNode<Control>(path_slider_max_volume).GetNode("HSlider") as HSlider;
        slider_time_scale = GetNode<Control>(path_slider_engine_time_scale).GetNode("HSlider") as HSlider;

        checkbox_no_flashing_lights.SetPressedNoSignal(Access.Instance.PreventFlashingLights);
        slider_rumble_strength.Value = Effects.Instance.RumbleStrength;
        slider_rumble_duration.Value = Effects.Instance.MaxRumbleDuration;
        slider_screen_shake_strength.Value = Effects.Instance.ScreenShakeStrength;
        slider_screen_shake_duration.Value = Effects.Instance.MaxScreenShakeDuration;
        slider_max_volume.Value = Access.Instance.AudioDecibelLimit;
        slider_time_scale.Value = Access.Instance.EngineTimeScale;
        option_font.Selected = Access.Instance.FontOption;

        checkbox_no_flashing_lights.Toggled += OnNoFlashingLightsChanged;
        option_font.ItemSelected += OnFontSelected;

        slider_rumble_strength.ValueChanged += SetRumbleStrength;
        slider_rumble_duration.ValueChanged += SetMaxRumbleDuration;
        slider_screen_shake_strength.ValueChanged += SetScreenShakeStrength;
        slider_screen_shake_duration.ValueChanged += SetMaxScreenShakeDuration;
        slider_max_volume.ValueChanged += SetMaxAudio;
        slider_time_scale.ValueChanged += SetEngineTimeScale;
        
    }

    private void OnNoFlashingLightsChanged(bool do_no_flashing_lights)
        => Access.Instance.PreventFlashingLights = do_no_flashing_lights;
    private void SetRumbleStrength(double value) 
        => Effects.Instance.RumbleStrength = (float)value;        
    private void SetScreenShakeStrength(double value) 
        => Effects.Instance.ScreenShakeStrength = (float)value;
    private void SetMaxRumbleDuration(double value) 
        => Effects.Instance.MaxRumbleDuration = (float)value;
    private void SetMaxScreenShakeDuration(double value) 
        => Effects.Instance.MaxScreenShakeDuration = (float)value;
    private void SetMaxAudio(double value) 
        => Access.Instance.AudioDecibelLimit = (float)value;
    private void SetEngineTimeScale(double value) 
        => Access.Instance.EngineTimeScale = (float)value;

    private void OnFontSelected(long index)
    {
        var target = option_font.GetItemId((int)index);
        if (target == Access.Instance.FontOption) return;

        Access.Instance.FontOption = target;
        FontHasChanged = true;
    }
    
    public void ApplyChanges()
    {
        Access.SaveSettings();
        Effects.SaveSettings();
        if (FontHasChanged) GetTree().ReloadCurrentScene();
    }
}
