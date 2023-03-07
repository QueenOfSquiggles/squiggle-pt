using Godot;
using queen.error;
using queen.extension;
using System;
using System.Collections.Generic;

public partial class PhaseTeleport : Node, PhaseBase
{

	private Marker3D rocking_chair;
	private Marker3D couch;
	private AnimationPlayer anim;
	private Node3D Actor;
	private Timer MoveTimer;
	private string LocationGroupName;

	private Node3D PlayerNode;

	private bool ActorIsVisible = false;

	public override void _Ready()
	{
		this.GetNode("MoveTimer", out MoveTimer);
		MoveTimer.Timeout += DoMove;
	}

	public void Setup(Marker3D p_rocking_chair, Marker3D p_couch, AnimationPlayer p_anim, Node3D actor_node, string loc_group_name)
    {
		rocking_chair = p_rocking_chair;
		couch = p_couch;
		anim = p_anim;
		Actor = actor_node;
		LocationGroupName = loc_group_name;
    }


    public void Start()
    {
		Print.Debug("Teardrop Entering phase: Teleport");
		PlayerNode = GetTree().GetFirstNodeInGroup("player") as Node3D;
		DoMove();
    }

    public void Stop()
    {
		MoveTimer.Stop();
    }

	public override void _Process(double _delta)
	{
		if (anim.CurrentAnimation == "Standing") LookAtPlayer();
	}

    private void DoMove()
    {
		if (ActorIsVisible)
		{
			// try again next time.
			MoveTimer.Start();
			return;
		}

		#if DEBUG
		if (PlayerNode == null)
		{
			Print.Error("Failed to acquire player node!");
			return;
		}
		#endif

		List<TeardropLocation> valid_targets = GetActiveTargets();
		TeardropLocation closest_marker = GetBestLocation(valid_targets);
		if (closest_marker == null)
		{
			Print.Error("Failed to find a valid marker!");
			MoveTimer.Start();
			return;
		} else {
			Print.Debug($"Attempting to teleport teardrop: {valid_targets.Count} valid positions found. ClosestMarker = {closest_marker}");
		}

		Actor.GlobalTransform = closest_marker.GlobalTransform;
		ManageAnimation(closest_marker);
		MoveTimer.Start();
    }

	private List<TeardropLocation> GetActiveTargets()
	{
		var targets = GetTree().GetNodesInGroup(LocationGroupName);
		List<TeardropLocation> valid_targets = new();
		Print.Debug($"Found {targets.Count} targets in tree");

		// prune to active locations with correct type
		foreach (var entry in targets)
		{
			if (entry is TeardropLocation tl && tl.IsActive) valid_targets.Add(tl);
		}
		return valid_targets;
	}

	private TeardropLocation GetBestLocation(List<TeardropLocation> valid_targets)
	{
		TeardropLocation closest_marker = null;
		float current_dist = float.MaxValue;
		foreach (var loc in valid_targets)
		{
			// use square distance for performance. Results are the same anyway
			var dist = (PlayerNode.GlobalPosition - loc.GlobalPosition).LengthSquared();
			if (dist < current_dist)
			{
				current_dist = dist;
				closest_marker = loc;
			}
		}
		Print.Debug($"Closest position was found {current_dist} meters away from the player");
		return closest_marker;
	}

	private void ManageAnimation(Marker3D location)
	{
		if (MatchLocation(location, rocking_chair)) anim.Play("RockingChair");
		else if (MatchLocation(location, couch)) anim.Play("Couch");
		else anim.Play("Standing");
	}

	private bool MatchLocation(Marker3D a, Marker3D b)
	{
		return (a.GlobalPosition - b.GlobalPosition).Length() < 0.1f;
	}

	private void LookAtPlayer()
	{
		var delta = PlayerNode.GlobalPosition - Actor.GlobalPosition;
		delta.Y = 0;
		Actor.LookAt(Actor.GlobalPosition + delta.Normalized(), Vector3.Up);
	}


	private void OnEnterVisiblity()
	{
		ActorIsVisible = true;
	}

	private void OnExitVisiblity()
	{
		ActorIsVisible = false;		
	}



}