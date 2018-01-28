using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightpathRenderer : MonoBehaviour
{
	[SerializeField]
	private LineRenderer _flightpathLinePrefab;
	private LineRenderer _flightpathLineRenderer; // Instance

	[SerializeField]
	private LineRenderer _flightpathConnectionLinePrefab;
	private LineRenderer _flightpathConnectionLineRenderer; // Instance

	private FlightController _flightController;
	

	GameObject _effectContainer = null;

	private void OnEnable()
	{
		_effectContainer = new GameObject("FlightpathEffectContainer");
		_flightpathLineRenderer = Instantiate(_flightpathLinePrefab, _effectContainer.transform);
		_flightpathConnectionLineRenderer = Instantiate(_flightpathConnectionLinePrefab, _effectContainer.transform);
	}

	private void OnDisable()
	{
		DestroyObject(_effectContainer);
		_effectContainer = null;
		_flightpathLineRenderer = null;
	}

	private void Start()
	{
		_flightController = GetComponent<FlightController>();
	}

	private void Update()
	{
		if (_flightController == null)
			return;

		Flightpath flightPath = _flightController.flightpath;
		LinkedListNode<Flightpath.Waypoint> currentWaypoint = _flightController.currentWaypoint;

		if (flightPath == null || currentWaypoint == null || !flightPath.isUserGenerated)
		{
			_flightpathLineRenderer.positionCount = 0;
			_flightpathConnectionLineRenderer.positionCount = 0;
			return;
		}

		// Draw connecting line from plane to path
		_flightpathConnectionLineRenderer.positionCount = 2;
		_flightpathConnectionLineRenderer.SetPosition(0, currentWaypoint.Value.Position);
		_flightpathConnectionLineRenderer.SetPosition(1, _flightController.transform.position);

		// Draw flight path line
		List<Vector3> points = new List<Vector3>(flightPath.waypointCount);
		for (LinkedListNode<Flightpath.Waypoint> wp = currentWaypoint; wp != null; wp = wp.Next)
		{
			points.Add(wp.Value.Position);
		}

		// Feed it the points backwards because the tiling UVs look nicer.
		Vector3[] pointsBackwards = new Vector3[points.Count];
		for (int i = 0; i < points.Count; i++)
		{
			pointsBackwards[(points.Count - 1) - i] = points[i];
		}
		_flightpathLineRenderer.positionCount = points.Count;
		_flightpathLineRenderer.SetPositions(pointsBackwards);
	}
}
