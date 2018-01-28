using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CreatePostLayers : MonoBehaviour
{
	public PostProcessResources resources;

	void Start ()
	{
		foreach (Camera camera in FindObjectsOfType<Camera>())
		{
			PostProcessLayer postLayer = camera.GetComponent<PostProcessLayer>();
			if (postLayer == null)
			{
				camera.allowHDR = true;
				camera.allowMSAA = false;
				postLayer = camera.gameObject.AddComponent<PostProcessLayer>();
				postLayer.Init(resources);
				postLayer.volumeLayer = 1 << LayerMask.NameToLayer("PostProcessing");

				postLayer.volumeTrigger = postLayer.transform;
			}
		}
	}
}
