using System;
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
	protected LinkedListNode<Flightpath.Waypoint> _currentWaypoint;
	protected Aeroplane _plane;
	protected Rigidbody _rb;

	const bool _drawDebugInfo = false;

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

	public LinkedListNode<Flightpath.Waypoint> currentWaypoint
	{
		get
		{
			return _currentWaypoint;
		}
	}

	public bool IsNull
	{
		get { return this == null; }
	}

	public void SetFlightpath(Flightpath flightpath)
	{
		if (_flightpath != flightpath)
		{
			_flightpath = flightpath;
			_navState = NavigationState.followingFlightPath;
		}
		_currentWaypoint = _flightpath.GetFirstWaypoint();
	}

	public void SetHoldingPattern(Vector3 location)
	{
		_flightpath = null;
		_currentWaypoint = null;
		_navState = NavigationState.holdingPattern;
		_holdingPatternLocation = transform.position;
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

	protected Vector3 GetUpDirection()
	{
		return Vector3.up;
	}

	protected bool IsWaypointInfront(WaypointInfo info)
	{
		return Vector3.Dot(info.worldDirection, transform.forward) > -0.2;
	}

	protected bool ShouldAdvanceWaypoint(WaypointInfo info)
	{
		return IsWaypointInfront(info) && (info.distance < _lookAheadDistance);
	}

	protected Vector3 GetBankedUpVector(Vector3 targetVector, float sensitivity, float maxBank)
	{
		Vector3 right = Vector3.Cross(transform.forward, GetUpDirection());
		float bankAmmount = Mathf.Clamp(Vector3.Dot(targetVector, right) * sensitivity, -1.0f, 1.0f);
		return Vector3.SlerpUnclamped(GetUpDirection(), right, bankAmmount * Mathf.Clamp01(maxBank));
	}

	protected void AdjustForWaypoint(WaypointInfo info)
	{
		_plane.targetFacing = info.worldDirection;
		_plane.targetUp = GetBankedUpVector(_plane.targetFacing, 5.0f, 0.5f);
		_plane.throttle = 1.0f;
	}

	protected void AdjustForHoldingPattern()
	{
		_plane.targetFacing = Vector3.Slerp(transform.forward, (_holdingPatternLocation - transform.position).normalized, 0.2f);
		_plane.targetUp = GetBankedUpVector(_plane.targetFacing, 3.0f, 0.2f);
		_plane.throttle = 0.5f;
	}

	private void Start()
	{
		_plane = GetComponent<Aeroplane>();
		_rb = GetComponent<Rigidbody>();
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
					if (_flightpath.isFinalized)
					{
						flightpath.OnPathExited(this);
						SetHoldingPattern(transform.position);
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

	public void OnLandingAttempted(bool wasSuccessful, LandingZone landingZone)
	{
		if(wasSuccessful)
		{
			Game.Instance.Mode.OnPlaneLanded();
		}

		foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
		{
			meshRenderer.material.color = wasSuccessful ? Color.green : Color.red;
		}
	}

	private void ResetColor()
	{
		foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
		{
			meshRenderer.material.color = Color.white;
		}
	}
}
