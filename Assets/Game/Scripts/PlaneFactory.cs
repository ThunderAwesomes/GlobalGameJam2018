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

		Debug.Log(result);

		return result;
	}

	public GameObject CreatePlane(GameObject mesh)
	{
		GameObject result = Instantiate(planeParent);
		GameObject meshObject = Instantiate(mesh);
		meshObject.transform.SetParent(result.transform);
		meshObject.transform.localPosition = Vector3.zero;


		foreach (Transform tf in meshObject.GetComponentsInChildren<Transform>())
		{
			if (tf.gameObject.name == "Propeller")
			{
				Debug.Log("This is a propeller");
				Rotator r = tf.gameObject.AddComponent<Rotator>();
				r.axis = Vector3.forward;
				r.speed = 1000;
			}
		}

		foreach (MeshCollider mc in meshObject.GetComponentsInChildren<MeshCollider>())
		{
			mc.convex = true;
		}

		return result;
	}

}
