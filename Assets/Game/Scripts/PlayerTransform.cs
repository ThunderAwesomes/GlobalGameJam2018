using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransform : MonoBehaviour
{
	[SerializeField]
	PlayerTransformRuntimeSet _runtimeSet;
	[SerializeField]
	private Transform _trackerAnchor;

	private void OnEnable()
	{
		_runtimeSet.Add(this);
		Reset();
	}

	private void OnDisable()
	{
		_runtimeSet.Remove(this);
	}

	private void Update()
	{
		_runtimeSet.playerHeight = _trackerAnchor.position.y + transform.lossyScale.y;
		_runtimeSet.position = _trackerAnchor.position;
		_runtimeSet.position.Scale(transform.lossyScale);
	}

	public void Reset()
	{
		transform.position = _runtimeSet.position;
		float scale = _runtimeSet.scale;
		transform.localScale = Vector3.one * scale;
	}

}
