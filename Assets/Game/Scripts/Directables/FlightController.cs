using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Aeroplane))]
public class FlightController : MonoBehaviour
{
	protected Flightpath _flightpath;
	protected LinkedListNode<Flightpath.Waypoint> _currentWaypoint;

	private Aeroplane plane;

	public Flightpath flightpath
	{
		get
		{
			return _flightpath;
		}
		set
		{
			_flightpath = value;
		}
	}

	protected bool ShouldAdvanceWaypoint()
	{
		// TODO: Implement
		return false;
	}

	private void Start()
	{
		plane = GetComponent<Aeroplane>();
	}

	private void Update()
	{
		while(ShouldAdvanceWaypoint())
		{
			_currentWaypoint = _currentWaypoint.Next;
		}

		// TODO: Adjust plane controls to fly towards waypoint
		plane.throttle = 1.0f;

	}

}
