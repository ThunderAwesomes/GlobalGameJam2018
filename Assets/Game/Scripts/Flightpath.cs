using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Flightpath
{
	const int smoothing = 5;

	public struct Waypoint
	{
		public Vector3 Position;

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

		Vector3 summedPosition = Vector3.zero;
		int iterations = smoothing < _sourcePoints.Count ? smoothing : _sourcePoints.Count;
		for (int i = 0; i < iterations; i++)
		{
			summedPosition += _sourcePoints[_sourcePoints.Count - 1 - i];
		}
		Vector3 averagedPosition = summedPosition / iterations;
		Waypoint waypoint = new Waypoint(averagedPosition);
		_waypoints.AddLast(waypoint);
	}

	public LinkedListNode<Waypoint> GetFirstWaypoint()
	{
		return _waypoints.First;
	}

	public void DrawDebug()
	{
		for (int i = 1; i < _sourcePoints.Count; i++)
		{
			Debug.DrawLine(_sourcePoints[i - 1], _sourcePoints[i], Color.red);
		}

		LinkedListNode<Flightpath.Waypoint> current = GetFirstWaypoint();
		LinkedListNode<Flightpath.Waypoint> next = current.Next;
		while (next != null)
		{
			Debug.DrawLine(current.Value.Position, next.Value.Position, Color.green);
			current = next;
			next = current.Next;
		}
	}
}
