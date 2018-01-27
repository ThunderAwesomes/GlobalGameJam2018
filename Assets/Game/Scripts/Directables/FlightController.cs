﻿using System.Collections;
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
			if (_flightpath != value && _flightpathRenderer != null)
			{
				_flightpathRenderer.SetFlightPath(_flightpath);
			}
			_flightpath = value;
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
		_plane.throttle = 0.0f;
	}

	private void Start()
	{
		_plane = GetComponent<Aeroplane>();
		_rb = GetComponent<Rigidbody>();
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
