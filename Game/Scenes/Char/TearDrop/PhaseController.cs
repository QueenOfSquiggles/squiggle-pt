using Godot;
using queen.extension;
using System;

public partial class PhaseController : Node
{
	[Export] private NodePath PathActorNode;
	[Export] private string LocationsGroupName = "TeardropLocation";
	[Export] private int StageIdle = 0;
	[Export] private int StageTeleport = 5;
	[Export] private int StageWeepingAngel = 6;
	
	private Node3D actor_node;

	private PhaseIdle idle;
	private PhaseTeleport teleport;
	private PhaseWeepingAngel angel;

	private PhaseBase CurrentBase = null;

	public override void _Ready()
	{
		this.GetNode(PathActorNode, out actor_node);
		// probably bad to assume indices but whatever
		idle = GetChild(0) as PhaseIdle;
		teleport = GetChild(1) as PhaseTeleport;
		angel = GetChild(2) as PhaseWeepingAngel;

		GameStages.StageChanged += OnStageChanged;
	
		OnStageChanged(GameStages.Current);
	}

	public override void _ExitTree()
	{
		GameStages.StageChanged -= OnStageChanged;
		CurrentBase.Stop();
	}

	public void Setup(Marker3D rocking_chair, Marker3D couch, AnimationPlayer anim)
	{
		idle.Setup(rocking_chair, couch, anim, actor_node);
		teleport.Setup(rocking_chair, couch, anim, actor_node, LocationsGroupName);
	}

	private void OnStageChanged(int stage)
	{
        PhaseBase next;
        if (stage >= StageWeepingAngel) next = angel;
		else if (stage >= StageTeleport) next = teleport;
		else next = idle;
		
		if (next != CurrentBase || CurrentBase == null)
		{
			CurrentBase?.Stop();
			CurrentBase = next;
			CurrentBase?.Start();
		}
	}

}
