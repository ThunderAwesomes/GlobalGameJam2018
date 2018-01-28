using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedPlaneFlightpath : MonoBehaviour
{
	[SerializeField]
	public PlaneFactory planeFactory;

	[SerializeField]
	public List<GameObject> nodes;

	private void Start()
	{
		GameObject plane = planeFactory.CreateRandomPlane(Vector3.zero, Quaternion.identity);

		IPathable pathable = plane.GetComponent<IPathable>();

		if(!pathable.IsNull() && nodes.Count > 0)
		{
			pathable.StartPath(nodes[0].transform.position);
			for(int i = 1; i < nodes.Count; i++)
			{
				pathable.AddPathPosition(nodes[i].transform.position);
			}
			pathable.EndPath();
		}
	}
}
