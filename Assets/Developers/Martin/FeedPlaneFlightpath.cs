using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedPlaneFlightpath : MonoBehaviour
{
	[SerializeField]
	public List<GameObject> nodes;

	public FlightController flightController;

	Flightpath path;

	private void Start()
	{
		path = new Flightpath(nodes[0].transform.position);
		for (int i = 1; i < nodes.Count; i++)
		{
			path.AddPosition(nodes[i].transform.position);
		}

		// Set flight controller's path.
		flightController.flightpath = path;

		path.finalized = true;
		
	}

	private void Update()
	{
		for (int i = 1; i < nodes.Count; i++)
		{
			Debug.DrawLine(nodes[i - 1].transform.position, nodes[i].transform.position, Color.red);
		}

		LinkedListNode<Flightpath.Waypoint> current = path.GetFirstWaypoint();
		LinkedListNode<Flightpath.Waypoint> next = current.Next;
		while (next != null)
		{
			Debug.DrawLine(current.Value.Position, next.Value.Position, Color.green);
			current = next;
			next = current.Next;
		}
	}
}
