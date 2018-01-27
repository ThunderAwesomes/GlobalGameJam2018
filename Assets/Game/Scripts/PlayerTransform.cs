using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransform : MonoBehaviour
{
	[SerializeField]
	PlayerTransformRuntimeSet _runtimeSet;

	private void OnEnable()
	{
		_runtimeSet.Add(this);
	}

	private void OnDisable()
	{
		_runtimeSet.Remove(this);
	}

}
