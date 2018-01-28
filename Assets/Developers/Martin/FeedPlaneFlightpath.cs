using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedPlaneFlightpath : MonoBehaviour
{
	[SerializeField]
	public PlaneFactory planeFactory;

	[SerializeField]
	public List<GameObject> nodes;

	Flightpath path;

	private void Start()
	{
		GameObject plane = planeFactory.CreateRandomPlane(Vector3.zero, Quaternion.identity);


		path = new Flightpath(nodes[0].transform.position);
		for (int i = 1; i < nodes.Count; i++)
		{
			path.AddPosition(nodes[i].transform.position);
		}

		// Set flight controller's path.
		plane.GetComponent<FlightController>().SetFlightpath(path);

		path.Finialized();
		
	}
}
