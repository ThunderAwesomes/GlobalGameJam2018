using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransform : MonoBehaviour
{
	[SerializeField]
	PlayerTransformRuntimeSet _runtimeSet;
	[SerializeField]
	private Transform _trackerAnchor;

	public float height
	{
		get { return _trackerAnchor.position.y * transform.lossyScale.y; }
	}


	private void OnEnable()
	{
		_runtimeSet.Add(this);
		Reset();
	}

	private void OnDisable()
	{
		_runtimeSet.Remove(this);
	}

	private void OnGUI()
	{
		GUILayout.Label("Height: " + height, GUI.skin.box);
	}

	public void Reset()
	{
		transform.position = _runtimeSet.position;
		float scale = _runtimeSet.scale;
		transform.localScale = Vector3.one * scale;
	}

}
