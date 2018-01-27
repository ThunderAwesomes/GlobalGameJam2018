using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Flightpath
{
	const int smoothing = 4;

	public struct Waypoint
	{
		Vector3 Position;

		public Waypoint(Vector3 position)
		{
			Position = position;
		}
	}

	private List<Vector3> _sourcePoints = new List<Vector3>();
	private LinkedList<Waypoint> _waypoints = new LinkedList<Waypoint>();

	public Flightpath(Vector3 startPosition)
	{
		Waypoint firstWaypoint = new Waypoint(startPosition);
		_waypoints.AddFirst(firstWaypoint);
	}

	public void AddPosition(Vector3 position)
	{
		_sourcePoints.Add(position);
		// TODO: Smoothing
		Waypoint waypoint = new Waypoint(position);
		_waypoints.AddLast(waypoint);
	}

	public LinkedListNode<Waypoint> GetFirstWaypoint()
	{
		return _waypoints.First;
	}
}
