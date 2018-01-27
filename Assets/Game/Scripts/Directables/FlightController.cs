using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Aeroplane), typeof(FlightpathRenderer), typeof(Rigidbody))]
public class FlightController : MonoBehaviour, IDirectable
{
	// Helper struct to provide some useful precomputed info about the waypoint in relation to ourselves in this instant.
	protected struct WaypointInfo
	{
		public float distance;
		public Vector3 worldDirection;
		public Vector3 localDirection;
		public Vector3 worldPosition;

	}

	protected enum NavigationState
	{
		holdingPattern,
		followingFlightPath,
		landing
	}

	protected NavigationState _navState;
	protected Flightpath _flightpath;
	protected Vector3 _holdingPatternLocation;
	protected FlightpathRenderer _flightpathRenderer;
	protected LinkedListNode<Flightpath.Waypoint> _currentWaypoint;
	protected Aeroplane _plane;
	protected Rigidbody _rb;

	const bool _drawDebugInfo = true;

	// Flight control tweakables
	private float _lookAheadDistance = 30.0f;

	// IDirectable Implementation
	public virtual Flightpath flightpath
	{
		get
		{
			return _flightpath;
		}
	}

	public void SetFlightpath(Flightpath flightpath)
	{
		if (_flightpath != flightpath)
		{
			_flightpath = flightpath;
			_flightpathRenderer.SetFlightPath(_flightpath);
			_navState = NavigationState.followingFlightPath;
		}

		if (_flightpath != null)
		{
			_currentWaypoint = _flightpath.GetFirstWaypoint();
		}
		else
		{
			_flightpathRenderer.ClearFlightPath();
			_currentWaypoint = null;
			_navState = NavigationState.holdingPattern;
			_holdingPatternLocation = transform.position;
		}
	}

	public void OnSelectionStateChanged(VRController.SelectionState state)
	{
		switch (state)
		{
			case VRController.SelectionState.Hover:
				foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
				{
					meshRenderer.material.color = Color.cyan;
				}
				break;
			case VRController.SelectionState.Select:
				foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
				{
					meshRenderer.material.color = Color.blue;
				}
				break;
			case VRController.SelectionState.None:
				foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
				{
					meshRenderer.material.color = Color.white;
				}
				break;
		}
	}
	// End IDirectable Implementations

	protected WaypointInfo ComputeWaypointInfo(Flightpath.Waypoint waypoint)
	{
		WaypointInfo info;
		info.distance = Vector3.Distance(transform.position, waypoint.Position);
		info.worldDirection = Vector3.Normalize(waypoint.Position - transform.position);
		info.localDirection = transform.InverseTransformDirection(info.worldDirection);
		info.worldPosition = waypoint.Position;
		return info;
	}

	protected Vector3 GetGravityDirection()
	{
		return Vector3.down;
	}

	protected bool IsWaypointInfront(WaypointInfo info)
	{
		return Vector3.Dot(info.worldDirection, transform.forward) > 0.2;
	}

	protected bool ShouldAdvanceWaypoint(WaypointInfo info)
	{
		return IsWaypointInfront(info) && (info.distance < _lookAheadDistance);
	}

	protected void AdjustForWaypoint(WaypointInfo info)
	{
		_plane.targetFacing = info.worldDirection;
		_plane.targetUp = -GetGravityDirection();

		// TODO: Adjust plane controls to fly towards waypoint
		_plane.throttle = 1.0f;
	}

	protected void AdjustForHoldingPattern()
	{
		_plane.targetFacing = (_holdingPatternLocation - transform.position).normalized;
		_plane.throttle = 0.5f;
	}

	private void Start()
	{
		_plane = GetComponent<Aeroplane>();
		_rb = GetComponent<Rigidbody>();
		_flightpathRenderer = GetComponent<FlightpathRenderer>();
	}

	private void Update()
	{
		if (_drawDebugInfo)
			DrawDebugInfo();

		if (_navState == NavigationState.holdingPattern)
		{
			AdjustForHoldingPattern();
		}
		else if (_navState == NavigationState.landing)
		{
			throw new System.NotImplementedException();
		}
		else
		{
			WaypointInfo info = ComputeWaypointInfo(_currentWaypoint.Value);

			while (ShouldAdvanceWaypoint(info))
			{
				LinkedListNode<Flightpath.Waypoint> nextWaypoint = _currentWaypoint.Next;

				if (nextWaypoint == null)
				{
					if (_flightpath.finalized)
					{
					  flightpath.consumed = true;
						SetFlightpath(null);
					}
					return;
				}

				_currentWaypoint = nextWaypoint;
				info = ComputeWaypointInfo(_currentWaypoint.Value);
			}

			AdjustForWaypoint(info);
		}
	}

	private void DrawDebugInfo()
	{
		if (_currentWaypoint != null)
		{
			Debug.DrawLine(transform.position, _currentWaypoint.Value.Position, Color.blue);
		}
	}

	private float debugFloat1;

	private void OnGUI()
	{
		if (_drawDebugInfo)
		{
			string DebugText = "";

			DebugText += "Angular veocity Y: " + _rb.angularVelocity.y + "\n";

			if (_currentWaypoint != null)
			{
				WaypointInfo info = ComputeWaypointInfo(_currentWaypoint.Value);
				DebugText += "Direction X: " + info.localDirection.x + "\n";
			}

			DebugText += "debugFloat1: " + debugFloat1 + "\n";

			GUILayout.Label(DebugText, GUI.skin.box);
		}
	}

}
