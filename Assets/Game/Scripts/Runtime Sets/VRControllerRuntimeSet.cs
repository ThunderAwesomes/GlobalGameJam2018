using System;
using UnityEngine;

[Serializable, CreateAssetMenu(menuName="Runtime Sets/VRController")]
public class VRControllerRuntimeSet : RuntimeSet<VRController>
{
	[SerializeField]
	private float _sphereCastRadius;
	[SerializeField]
	private float _waypointDelta;
	[SerializeField]
	private bool _isDebug;

	/// <summary>
	/// How big the physics sphere cast will be when using the 
	/// trigger to select <see cref="IDirectable"/>s.
	/// </summary>
	public float sphereCastRadius
	{
		get { return _sphereCastRadius; }
	}

	/// <summary>
	/// Gets the delta the control must move to trigger making a new way point 
	/// </summary>
	public float waypointDelta
	{
		get { return _waypointDelta; }
	}

	/// <summary>
	/// If true we will draw debug lines on the controller 
	/// </summary>
	public bool isDebug
	{
		get { return _isDebug; }
		set { _isDebug = value; }
	}
}
