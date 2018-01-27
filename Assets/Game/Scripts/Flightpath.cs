using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Flightpath : MonoBehaviour
{
	public struct Waypoint
	{
		Vector3 position;
	}

	private LinkedList<Waypoint> _waypoints = new LinkedList<Waypoint>();

	public Flightpath(Flightpath.Waypoint initialWaypoint)
	{
		_waypoints.AddFirst(initialWaypoint);
	}
}
