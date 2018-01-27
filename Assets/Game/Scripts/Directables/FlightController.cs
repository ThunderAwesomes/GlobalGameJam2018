using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour
{
	protected LinkedListNode<Flightpath.Waypoint> currentWaypoint;

	protected bool ShouldAdvanceWaypoint()
	{
		throw new System.NotImplementedException();
	}

	private void Update()
	{
		while(ShouldAdvanceWaypoint())
		{
			currentWaypoint = currentWaypoint.Next;
		}

		// TODO: Adjust plane controls to fly towards waypoint
	}

}
