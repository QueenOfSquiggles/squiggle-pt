using Godot;
using queen.error;
using System;

public partial class PhaseIdle : Node, PhaseBase
{

	[Export] int StageSwitchToCouch = 4;
	private Marker3D rocking_chair;
	private Marker3D couch;
	private AnimationPlayer anim;
	private Node3D Actor;

    public void Setup(Marker3D p_rocking_chair, Marker3D p_couch, AnimationPlayer p_anim, Node3D actor_node)
    {
		rocking_chair = p_rocking_chair;
		couch = p_couch;
		anim = p_anim;
		Actor = actor_node;
		OnStageChanged(GameStages.Current); // This will usually get called at the appropriate time
    }

    public void Start()
    {
		Print.Debug("Teardrop Entering phase: Idle");
		GameStages.StageChanged += OnStageChanged;
		OnStageChanged(GameStages.Current);
    }

    public void Stop()
    {
		GameStages.StageChanged -= OnStageChanged;
	}

	private void OnStageChanged(int stage)
	{
		if (stage < StageSwitchToCouch) MoveTo(rocking_chair, "Chair");
		else MoveTo(couch, "Couch");
	}

	private void MoveTo(Marker3D position, string anim_name)
	{
		if (position == null) return;

		Actor.GlobalTransform = position.GlobalTransform;
		anim.Play(anim_name);
	}

    public bool CanKillPlayer() => false;
}
