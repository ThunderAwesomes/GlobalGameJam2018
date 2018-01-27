using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Aeroplane), typeof(FlightpathRenderer))]
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
	protected FlightpathRenderer _flightpathRenderer;
	protected LinkedListNode<Flightpath.Waypoint> _currentWaypoint;
	private Aeroplane _plane;

	const bool _drawDebugInfo = true;

	// Flight control tweakables
	private float _lookAheadDistance = 40.0f;

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
		}
	}

	public virtual void OnHover()
	{
		foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
		{
			meshRenderer.material.color = Color.cyan;
		}
	}

	public virtual void OnSelected()
	{
		foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
		{
			meshRenderer.material.color = Color.blue;
		}
	}

	public virtual void OnDeselected()
	{
		foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
		{
			meshRenderer.material.color = Color.white;
		}
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
		// Compute a left-right steering value.
		float Steering = 0;
		if (info.localDirection.z > 0)
		{
			// Waypoint is infront
			Steering = Mathf.Clamp(info.localDirection.x, -1.0f, 1.0f);
		}
		else
		{
			// Waypoint is behind
			Steering = Mathf.Sign(info.localDirection.x);
		}


		// Convert that to a roll and yaw input



		_plane.yaw = Steering;

		// TODO: Adjust plane controls to fly towards waypoint
		_plane.throttle = 1.0f;
	}

	protected void AdjustForHoldingPattern()
	{
		_plane.throttle = 0.0f;
	}

	private void Start()
	{
		_plane = GetComponent<Aeroplane>();
		_flightpathRenderer = GetComponent<FlightpathRenderer>();
	}

	private void Update()
	{
		if (flightpath == null || _currentWaypoint == null)
			return;

		WaypointInfo info = ComputeWaypointInfo(_currentWaypoint.Value);

		while (ShouldAdvanceWaypoint(info))
		{
			_currentWaypoint = _currentWaypoint.Next;

			if (_currentWaypoint == null)
			{
				_flightpath = null;
				break;
			}

			info = ComputeWaypointInfo(_currentWaypoint.Value);
		}

		if (_currentWaypoint != null)
		{
			AdjustForWaypoint(info);
		}
		else
		{
			AdjustForHoldingPattern();
		}

		if (_drawDebugInfo)
			DrawDebugInfo();
	}

	private void DrawDebugInfo()
	{
		if (_currentWaypoint != null)
		{
			Debug.DrawLine(transform.position, _currentWaypoint.Value.Position, Color.blue);
		}
	}

}
