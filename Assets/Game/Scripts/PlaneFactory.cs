using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlaneFactory", menuName = "PlaneGame/NewPlaneFactory")]
public class PlaneFactory : ScriptableObject
{
	[SerializeField]
	protected int seed = 0;
	[SerializeField]
	protected GameObject planeParent;
	[SerializeField]
	protected List<GameObject> planeMeshes;

	public GameObject CreateRandomPlane(Vector3 position, Quaternion rotation)
	{
		int index = 0;
		if (Application.isPlaying)
		{
			Random.InitState(seed);
			int nextSeed = Random.Range(0, int.MaxValue);
			index = Random.Range(0, planeMeshes.Count - 1);
			seed = nextSeed;
		}
		else
		{
			index = Random.Range(0, planeMeshes.Count - 1);
		}

		GameObject result = CreatePlane(planeMeshes[index]);

		result.transform.position = position;
		result.transform.rotation = rotation;

		return result;
	}

	public GameObject CreatePlane(GameObject mesh)
	{
		GameObject result = GameObject.Instantiate(planeParent);
		GameObject meshObject = GameObject.Instantiate(mesh);
		meshObject.transform.SetParent(result.transform);
		meshObject.transform.localPosition = Vector3.zero;

		// Get plane stats from mesh name
		MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
		string statString = meshFilter.sharedMesh.name;

		foreach(var pair in statString.Split(','))
		{
			var splitPait = pair.Split(':');
			
			switch(splitPait[0])
			{
				//case:
			}
		}

		return result;
	}

}
