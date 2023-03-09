using Godot;
using queen.error;
using queen.extension;
using System;

public partial class TearDropChar : CharacterBody3D
{

	[ExportCategory("External")]
	[Export] private NodePath path_location_rocking_chair;
	[Export] private NodePath path_location_couch;


	[ExportCategory("Internal")]
	[Export] private NodePath path_phase_controllers;
	[Export] private NodePath path_anim_driver;


	// Acquired Nodes
	private Marker3D location_rocking_chair;
	private Marker3D location_couch;
	private PhaseController phase_controller;
	private AnimationPlayer anim;

	public override void _Ready()
	{
		this.GetNode(path_phase_controllers, out phase_controller);
		this.GetNode(path_anim_driver, out anim);
		this.GetNode(path_location_rocking_chair, out location_rocking_chair);
		this.GetNode(path_location_couch, out location_couch);

		phase_controller.Setup(location_rocking_chair, location_couch, anim);
	}


}
