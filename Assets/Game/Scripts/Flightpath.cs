using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class Flightpath : IEnumerable<Flightpath.Waypoint>
{
	public delegate void FlightPathDelegate(Flightpath flightPath);
	public delegate void DirectableDelegate(Flightpath flightPath, IDirectable IDirectable);


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

	private List<Vector3> _sourcePoints;
	private LinkedList<Waypoint> _waypoints;
	private bool _drawPath;
	private bool _isFinalized;
	private bool _disposeOnComplete;
	private int _occupancy;
	private float _lookAheadDistance = 30f;
	private event DirectableDelegate _onPathStarted;
	private event DirectableDelegate _onPathExited;
	private event FlightPathDelegate _onPathFinalized;

	public bool drawPath
	{
		get { return _drawPath; }
		set { _drawPath = value; }
	}

	public float lookAheadDistance
	{
		get { return _lookAheadDistance; }
		set { _lookAheadDistance = value; }
	}

	public int occupancy
	{
		get { return _occupancy; }
	}

	public bool isFinalized
	{
		get { return _isFinalized; }
	}
	public int count
	{
		get { return _sourcePoints.Count; }
	}

	public bool disposeOnComplete
	{
		get { return _disposeOnComplete; }
		set { _disposeOnComplete = value; }
	}

	public Vector3 this[int index]
	{
		get
		{
			return _sourcePoints[index];
		}
	}

	public event DirectableDelegate onPathStarted
	{
		add { _onPathStarted += value; }
		remove { _onPathStarted -= value; }
	}

	public event DirectableDelegate onPathExited
	{
		add { _onPathExited += value; }
		remove { _onPathExited -= value; }
	}

	public event FlightPathDelegate onPathFinalized
	{
		add { _onPathFinalized += value; }
		remove { _onPathFinalized -= value; }
	}

	private Flightpath()
	{
		_sourcePoints = new List<Vector3>();
		_waypoints = new LinkedList<Waypoint>();
		_drawPath = true;
		_disposeOnComplete = true;
	}

	public Flightpath(Vector3 startPosition) : this()
	{
		AddPosition(startPosition);
	}

	public void Reset()
	{
		_isFinalized = false;
		_waypoints.Clear();
		_sourcePoints.Clear();
	}

	/// <summary>
	/// Invoked when a directable starts moving along
	/// this path.
	/// </summary>
	public void OnPathStarted(IDirectable iDirectable)
	{
		_occupancy++;
		if (_onPathStarted != null)
		{
			_onPathStarted(this, iDirectable);
		}
	}


	/// <summary>
	/// Invoked when this path has been completed or
	///  they left it 
	/// </summary>
	public void OnPathExited(IDirectable iDirectable)
	{
		_occupancy--;
		if (_onPathExited != null)
		{
			_onPathExited(this, iDirectable);
		}

		if (_occupancy <= 0)
		{
			// Destroy 
		}
	}

	/// <summary>
	/// Invoked when our path has been finialized.
	/// </summary>
	public void Finialized()
	{
		_isFinalized = true;
		if (_onPathFinalized != null)
		{
			_onPathFinalized(this);
		}
	}

	/// <summary>
	/// Adds a new position along the flight path. 
	/// </summary>
	public void AddPosition(Vector3 position)
	{
		if (_isFinalized)
		{
			throw new InvalidOperationException("You can't add points to a finalized path");
		}
		if (_waypoints.Count == 0)
		{
			Waypoint firstWaypoint = new Waypoint(position);
			_waypoints.AddFirst(firstWaypoint);
		}
		else
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
	}



	public LinkedListNode<Waypoint> GetFirstWaypoint()
	{
		return _waypoints.First;
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
