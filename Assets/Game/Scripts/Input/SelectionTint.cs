using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tint
{
	public static void ByState(SelectionState state, GameObject gameObject)
	{
		Color color = Color.white;

		switch (state)
		{
			case SelectionState.Highlighted:
				color = Color.cyan;
				break;
			case SelectionState.Pressed:
				color = Color.blue;
				break;
			case SelectionState.None:
				color = Color.white;
				break;
		}

		foreach (MeshRenderer meshRenderer in gameObject.GetComponentsInChildren<MeshRenderer>())
		{
			meshRenderer.material.color = color;
		}
	}
}
