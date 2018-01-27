using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Aeroplane))]
public class FlightController : MonoBehaviour, IDirectable
{
	// Helper struct to provide some useful precomputed info about the waypoint in relation to ourselves in this instant.
	protected struct WaypointInfo
	{
		public float distance;
		public Vector3 direction;
		public Vector3 position;
	}

	protected Flightpath _flightpath;
	protected LinkedListNode<Flightpath.Waypoint> _currentWaypoint;

	private Aeroplane plane;

	const bool _drawDebugInfo = true;

	// Flight control tweakables
	private float lookAheadDistance = 20.0f;

	// IDirectable Implementation
	public virtual Flightpath flightpath
	{
		get
		{
			return _flightpath;
		}
		set
		{
			_flightpath = value;
			_currentWaypoint = _flightpath.GetFirstWaypoint();
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
		info.direction = Vector3.Normalize(transform.position - waypoint.Position);
		info.position = waypoint.Position;
		return info;
	}

	protected bool IsWaypointInfront(WaypointInfo info)
	{
		return Vector3.Dot(info.direction, transform.forward) > 0.2;
	}

	protected bool ShouldAdvanceWaypoint(WaypointInfo info)
	{
		return IsWaypointInfront(info) || (info.distance < lookAheadDistance);
	}

	protected void AdjustForWaypoint(WaypointInfo info)
	{
		// TODO: Adjust plane controls to fly towards waypoint
		plane.throttle = 1.0f;
	}

	protected void AdjustForHoldingPattern()
	{
		plane.throttle = 0.0f;
	}

	private void Start()
	{
		plane = GetComponent<Aeroplane>();
	}

	private void Update()
	{
		if (flightpath == null || _currentWaypoint == null)
			return;

		WaypointInfo info = ComputeWaypointInfo(_currentWaypoint.Value);

		while(ShouldAdvanceWaypoint(info))
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
