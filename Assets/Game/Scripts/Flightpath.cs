using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Flightpath : IEnumerable<Flightpath.Waypoint>
{
	const int smoothing = 5;

	public struct Waypoint
	{
		public Vector3 Position;

		public Waypoint(Vector3 position)
		{
			Position = position;
		}

		public Waypoint(Vector3[] positions)
		{
			Vector3 summedPosition = Vector3.zero;
			for (int i = 0; i < positions.Length; i++)
			{
				summedPosition += positions[i];
			}
			Vector3 averagedPosition = summedPosition / positions.Length;

			Position = averagedPosition;
		}
	}


	public int waypointCount
	{
		get { return _waypoints.Count; }
	}

	private List<Vector3> _sourcePoints = new List<Vector3>();
	private LinkedList<Waypoint> _waypoints = new LinkedList<Waypoint>();
	public bool finalized { get; set; }

	public Flightpath(Vector3 startPosition)
	{
		Waypoint firstWaypoint = new Waypoint(startPosition);
		_waypoints.AddFirst(firstWaypoint);
	}

	public void AddPosition(Vector3 position)
	{
		_sourcePoints.Add(position);

		int iterations = smoothing < _sourcePoints.Count ? smoothing : _sourcePoints.Count;
		Vector3[] points = new Vector3[iterations];
		for (int i = 0; i < points.Length; i++)
		{
			points[i] = _sourcePoints[_sourcePoints.Count - 1 - i];
		}
		Waypoint waypoint = new Waypoint(points);
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
			//GL.PushMatrix();
			//mat.SetPass(0);
			//GL.LoadOrtho();
			GL.Begin(GL.LINES);
			GL.Color(Color.red);
			GL.Vertex(current.Value.Position);
			GL.Vertex(next.Value.Position);
			GL.End();
			//GL.PopMatrix();
			current = next;
			next = current.Next;
		}
	}

	public IEnumerator<Waypoint> GetEnumerator()
	{
		LinkedListNode<Waypoint> current = _waypoints.First;

		while (current != null)
		{
			yield return current.Value;
			current = current.Next;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		yield return GetEnumerator();
	}
}
