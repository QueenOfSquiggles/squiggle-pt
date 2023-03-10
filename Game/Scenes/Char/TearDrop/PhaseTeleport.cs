using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using queen.error;
using queen.extension;

public partial class PhaseTeleport : Node, PhaseBase
{

	[Export] private float WaitTimeLong = 30.0f;
	[Export] private float WaitTimeShort = 2.0f;
	[Export] private float ChanceExactLocation = 0.25f;
	private TeardropLocation rocking_chair;
	private TeardropLocation couch;
	private AnimationPlayer anim;
	private Node3D Actor;
	private Timer MoveTimer;
	private string LocationGroupName;

	private Node3D PlayerNode;

	private bool ActorIsVisible = false;
	private readonly Random random = new();
	private bool KillingEnabled = true;
	private bool IsActive = false;

	public override void _Ready()
	{
		this.GetNode("MoveTimer", out MoveTimer);
		MoveTimer.Timeout += DoMove;
	}

	public void Setup(TeardropLocation p_rocking_chair, TeardropLocation p_couch, AnimationPlayer p_anim, Node3D actor_node, string loc_group_name)
    {
		rocking_chair = p_rocking_chair;
		couch = p_couch;
		anim = p_anim;
		Actor = actor_node;
		LocationGroupName = loc_group_name;
    }


    public void Start()
    {
		IsActive = true;
		// Print.Debug("Teardrop Entering phase: Teleport");
		PlayerNode = GetTree().GetFirstNodeInGroup("player") as Node3D;
		DoMove();
    }

    public void Stop()
    {
		MoveTimer.Stop();
		IsActive = false;
    }


	public override void _Process(double _delta)
	{
		if (IsActive && anim.CurrentAnimation == "Standing") LookAtPlayer();
	}



    private void DoMove()
    {
		if (ActorIsVisible)
		{
			// try again next time.
			MoveTimer.Start(WaitTimeShort);
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
			MoveTimer.Start(WaitTimeLong);
			return;
		} else {
			// Print.Debug($"Attempting to teleport teardrop: {valid_targets.Count} valid positions found. ClosestMarker = {closest_marker.GlobalPosition}");
		}

		PlayerSafetyNet();
		Actor.GlobalTransform = closest_marker.GlobalTransform;
		ManageAnimation(closest_marker);
		MoveTimer.Start(WaitTimeLong);
    }

	private async void PlayerSafetyNet()
	{
		// temporary disable the kill box on teleport to avoid insta-killing player
		// should make it a bit more fair, while still being spooky.
		KillingEnabled = false;
		await Task.Delay(500);
		KillingEnabled = true;
	}

	private List<TeardropLocation> GetActiveTargets()
	{
		var targets = GetTree().GetNodesInGroup(LocationGroupName);
		List<TeardropLocation> valid_targets = new();
		// Print.Debug($"Found {targets.Count} targets in tree");

		// prune to active locations with correct type
		foreach (var entry in targets)
		{
			if (entry is TeardropLocation tl && tl.IsActive && tl.IsStandingPosition && (tl.GlobalPosition - Actor.GlobalPosition).LengthSquared() > 0.1) 
			{
				valid_targets.Add(tl);
			}
		}
		return valid_targets;
	}

	private TeardropLocation GetBestLocation(List<TeardropLocation> valid_targets)
	{
        // If we win the random, go to perfect spot. Else choose a random location.
        // Ideally this will prevent Teardrop from camping a specific spot, while still moving erratically.
        if (random.NextSingle() < ChanceExactLocation) return GetClosestLocation(valid_targets);
		return valid_targets[random.Next() % valid_targets.Count];
	}

	private TeardropLocation GetClosestLocation(List<TeardropLocation> valid_targets)
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
		return closest_marker;
	}

	private void ManageAnimation(TeardropLocation location)
	{
		if (!anim.HasAnimation(location.AnimKey))
		{
            Print.Error($"Failed to play location animation for Teardrop: '{location.AnimKey}'");
        }
        anim.Play(location.AnimKey);
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

    public bool CanKillPlayer() => KillingEnabled;

}