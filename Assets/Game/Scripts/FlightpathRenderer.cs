using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FlightpathRenderer : MonoBehaviour
{
	private FlightController _flightController;
	private LineRenderer _lineRenderer;

	private void Start()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_flightController = GetComponent<FlightController>();
	}

	private void Update()
	{
		Flightpath flightPath = _flightController.flightpath;

		if (flightPath == null || !flightPath.isUserGenerated)
		{
			_lineRenderer.positionCount = 0;
			return;
		}

		if (flightPath.waypointCount != _lineRenderer.positionCount)
		{
			_lineRenderer.positionCount = flightPath.waypointCount;
			int index = 0;

			foreach (Flightpath.Waypoint waypoint in flightPath)
			{
				_lineRenderer.SetPosition(index, waypoint.Position);
				index++;
			}
		}
	}
}
