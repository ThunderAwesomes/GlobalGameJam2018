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
		set
		{
			if (_flightpath != value)
			{
				_flightpath = value;
				_flightpathRenderer.SetFlightPath(_flightpath);
			}

			if (_flightpath != null)
			{
				_currentWaypoint = _flightpath.GetFirstWaypoint();
			}
			else
			{
				_currentWaypoint = null;
			}
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

	public void EnterHoldingPattern()
	{
		flightpath.consumed = true;
		flightpath = null;
		_flightpathRenderer.ClearFlightPath();
		_holdingPatternLocation = transform.position;
	}

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

	float GetSteeringInput(float angularDistance, float angularVelocity)
	{
		float result = Mathf.Clamp(angularDistance, -1.0f, 1.0f);

		// Reduce steering if we're already moving towards the target.
		if (Mathf.Sign(angularVelocity) == Mathf.Sign(angularDistance))
		{
			float distance = Mathf.Abs(angularDistance);
			float speed = Mathf.Abs(angularVelocity);

			float steeringMultiplier = Mathf.Clamp01((distance - speed) * 5);


			result *= steeringMultiplier;
		}

		return result;
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

		if (flightpath == null || _currentWaypoint == null)
		{
			AdjustForHoldingPattern();
			return;
		}

		WaypointInfo info = ComputeWaypointInfo(_currentWaypoint.Value);

		while (ShouldAdvanceWaypoint(info))
		{
			LinkedListNode<Flightpath.Waypoint> nextWaypoint = _currentWaypoint.Next;

			if (nextWaypoint == null)
			{
				if (_flightpath.finalized)
				{
					EnterHoldingPattern();
				}
				return;
			}

			_currentWaypoint = nextWaypoint;
			info = ComputeWaypointInfo(_currentWaypoint.Value);
		}

		AdjustForWaypoint(info);
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
