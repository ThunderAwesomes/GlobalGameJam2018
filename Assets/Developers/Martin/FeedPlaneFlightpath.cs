using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedPlaneFlightpath : MonoBehaviour
{
	[SerializeField]
	public PlaneFactory planeFactory;

	[SerializeField]
	public List<GameObject> nodes;

	[SerializeField]
	private int _spawnCount;

	private IEnumerator Start()
	{
		for (int i = 0; i < _spawnCount; i++)
		{
			GameObject plane = planeFactory.CreateRandomPlane(Vector3.zero, Quaternion.identity);

			IPathable pathable = plane.GetComponent<IPathable>();

			if (!pathable.IsNull() && nodes.Count > 0)
			{
				pathable.StartPath(nodes[0].transform.position);
				for (int x = 1; x < nodes.Count; x++)
				{
					pathable.AddPathPosition(nodes[x].transform.position);
				}
				pathable.EndPath();
			}
			yield return new WaitForSeconds(1f);
		}
	}
}
