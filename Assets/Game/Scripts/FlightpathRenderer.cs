using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FlightpathRenderer : MonoBehaviour
{
	[SerializeField]
	private FlightPathRendererRuntimeSet _runtimeSet;

	private Flightpath _flightPath;
	private LineRenderer _lineRenderer;

	public void SetFlightPath(Flightpath flightPath)
	{
		_flightPath = flightPath;
	}

	private void OnEnable()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_runtimeSet.Add(this);
	}

	private void OnDisable()
	{
		_runtimeSet.Remove(this);
	}

	private void Update()
	{
		if (_flightPath != null)
		{
			if (_flightPath.waypointCount != _lineRenderer.positionCount)
			{
				_lineRenderer.positionCount = _flightPath.waypointCount;
				int index = 0;

				foreach (Flightpath.Waypoint waypoint in _flightPath)
				{
					_lineRenderer.SetPosition(index, waypoint.Position);
					index++;
				}
			}
		}
	}
}
