using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using queen.error;
using queen.extension;

public partial class PhaseWeepingAngel : Node, PhaseBase
{
    [Export] private NodePath path_nav_agent;
    [Export] private NodePath PathRayCast;
    [Export] private NodePath PathObjectAvoidRayLeft;
    [Export] private NodePath PathObjectAvoidRayRight;
    [Export] private float MoveSpeed = 1.0f;
    [Export] private float TurnSpeed = 1.0f;
    [Export] private float UpdateDistance = 2.0f;
    [Export] private float ObjectAvoidanceScalar = 1.0f;
    [Export] private int OnOffScreenDelay = 500;

    private float InternalUpdateDistance;

    private NavigationAgent3D nav_agent;
    private Node3D PlayerNode;
    private CharacterBody3D Actor;
    private AnimationPlayer anim;
    private string LocationsGroupName;
    private bool IsActive = false;
    private Timer timer;
    private RayCast3D raycast;
    private RayCast3D ray_obj_left;
    private RayCast3D ray_obj_right;
    private bool IsVisible = false;

    public override void _Ready()
    {
        this.GetNode(path_nav_agent, out nav_agent);
        this.GetNode("Timer", out timer);
        this.GetNode(PathRayCast, out raycast);
        this.GetNode(PathObjectAvoidRayLeft, out ray_obj_left);
        this.GetNode(PathObjectAvoidRayRight, out ray_obj_right);
        InternalUpdateDistance = Mathf.Pow(UpdateDistance, 2.0f); // square to save processing time.
    }

    public void Setup(AnimationPlayer p_anim, CharacterBody3D actor_node, string locations_group_name)
    {
		anim = p_anim;
		Actor = actor_node;
        LocationsGroupName = locations_group_name;
    }

    public void Start()
    {
        PlayerNode = GetTree().GetFirstNodeInGroup("player") as Node3D;
        nav_agent.TargetPosition = PlayerNode.GlobalPosition;
        IsActive = true;
        anim.Play("Crawl");
        MoveToValidStart();
        timer.Start();
    }

    public void Stop()
    {
        timer.Stop();
        IsActive = false;
    }

    private void MoveToValidStart()
	{
        var locations = GetTree().GetNodesInGroup(LocationsGroupName);
        List<TeardropLocation> targets = new();
        foreach (var entry in locations)
        {
            if (entry is TeardropLocation tl && tl.IsStandingPosition) targets.Add(tl);
        }
        TeardropLocation closest = null;
        float closest_dist = float.MaxValue;
        foreach (var entry in targets)
        {
            float dist = (Actor.GlobalPosition - entry.GlobalPosition).LengthSquared();
            if (dist < closest_dist)
            {
                closest = entry;
                closest_dist = dist;
            }
        }
        if (closest == null) Print.Error("Failed to find valid closest target for setting up weeping angel phase");
        else Actor.GlobalPosition = closest.GlobalPosition;
    }


    public override void _PhysicsProcess(double _d)
    {
        if (!IsActive) return;
        if (IsVisible) return; // don't move while visible
        if ((nav_agent.TargetPosition - PlayerNode.GlobalPosition).LengthSquared() >= InternalUpdateDistance)
            UpdateTarget();
        if (CheckForDoors()) 
        {
            Actor.Velocity = Vector3.Zero;
            return; // wait for door to open
        }
        var next = nav_agent.GetNextPathPosition();
        var delta = next - Actor.GlobalPosition;
        delta.Y = 0;
        delta = delta.Normalized();
        Actor.Velocity = delta * MoveSpeed;
        Actor.Velocity += GetObjectAvoidance();
        if (!Actor.IsOnFloor()) Actor.Velocity += Vector3.Down * 2.0f;
        Actor.MoveAndSlide();
        UpdateLookDirection(-delta, (float)_d);
    }

    private Vector3 GetObjectAvoidance()
    {
        Vector2 dists = Vector2.One * 5.0f; // assume massive value otherwise
        if (ray_obj_left.IsColliding())
            dists.X = (ray_obj_left.GetCollisionPoint() - Actor.GlobalPosition).Length();
        if (ray_obj_right.IsColliding())
            dists.Y = (ray_obj_right.GetCollisionPoint() - Actor.GlobalPosition).Length();

        dists = dists.Normalized();
        if (Mathf.Abs(dists.Y - dists.X) < 0.1f) return Vector3.Zero; // close to equal, no avoidance needed

        if (dists.Y > dists.X) // More room to the right, tend that way
            return Actor.GlobalTransform.Right() * ObjectAvoidanceScalar;

        // More room to the left, tend that way
        return Actor.GlobalTransform.Left() * ObjectAvoidanceScalar;
    }

    private bool CheckForDoors()
	{
		if(!raycast.IsColliding()) return false;
        var coll = raycast.GetCollider() as Node3D;
        if (!coll.IsInGroup("door")) return false;
        if (coll is DoorRigidBody drb && !drb.IsOpen)
        {
            drb.RequestOpen();
            return true;
        }
        return false; // door is actually already open, we are just colliding for some reason?
    }
    private void UpdateTarget()
    {
        if (!IsActive) return;
        nav_agent.TargetPosition = PlayerNode.GlobalPosition;
    }

    private void UpdateLookDirection(Vector3 dir, float delta)
    {
        var rot = Actor.GlobalRotation;
        rot.Y = Mathf.LerpAngle(rot.Y, dir.AngleXZ(), TurnSpeed * delta);
        Actor.GlobalRotation = rot;
    }


    public bool CanKillPlayer() => true;

    public async void OnEnterScreen()
    {
        if (!IsActive) return;
        await Task.Delay(OnOffScreenDelay); // give a moment
        IsVisible = true;
        anim.Pause();
    }

    public async void OnExitScreen() { 
        if (!IsActive) return;
        await Task.Delay(OnOffScreenDelay);
        IsVisible = false;
        anim.Play("Crawl");
    }

}