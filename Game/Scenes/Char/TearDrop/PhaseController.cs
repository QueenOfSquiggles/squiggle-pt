using System;
using Godot;
using queen.error;
using queen.extension;

public partial class PhaseController : Node
{
	[Export] private NodePath PathActorNode;

    [Export] private string LocationsGroupName = "TeardropLocation";
	[Export] private int StageIdle = 0;
	[Export] private int StageTeleport = 5;
	[Export] private int StageWeepingAngel = 8;

	[Export] private PackedScene JumpscareScene;
	[Export] private bool DisableDeath = false;
	
	private CharacterBody3D actor_node;

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

	public void Setup(TeardropLocation rocking_chair, TeardropLocation couch, AnimationPlayer anim)
	{
		idle.Setup(rocking_chair, couch, anim, actor_node);
		teleport.Setup(rocking_chair, couch, anim, actor_node, LocationsGroupName);
		angel.Setup(anim, actor_node, LocationsGroupName);
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

	public void OnBodyEnterKillbox(Node3D body)
	{
		if (!body.IsInGroup("player")) return;		
		if(!CurrentBase.CanKillPlayer()) return;
		if (DisableDeath) return;
        DoJumpscare();
    }

	private void DoJumpscare()
	{
        var node = JumpscareScene.Instantiate();
		var brain = GetTree().GetFirstNodeInGroup("cam_brain") as CameraBrain;
		if (brain == null) Print.Error("Failed to find Camera Brain in scene!!!");
     	brain.AddChild(node);
        actor_node.QueueFree();
    }

	#if DEBUG
	public override void _Input(InputEvent e)
	{
		if (e is InputEventKey kp && kp.Keycode == Key.Kp4)
		{
            DoJumpscare();
        }
	}
	#endif

	

}
