using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SelectionBounds : MonoBehaviour
{
	private BoxCollider _boxCollider;

	// Use this for initialization
	public void Start()
	{
		_boxCollider = GetComponent<BoxCollider>();
		_boxCollider.isTrigger = true;
		CalculateBounds();
	}

	/// <summary>
	/// Calculates the bounds around an object 
	/// </summary>
	public void CalculateBounds()
	{
		Collider[] colliders = GetComponentsInChildren<Collider>();

		bool hasBounds = false;
		Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

		for (int i = 0; i < colliders.Length; ++i)
		{
			if (hasBounds)
			{
				bounds.Encapsulate(colliders[i].bounds);
			}
			else
			{
				bounds = colliders[i].bounds;
				hasBounds = true;
			}
		}

		_boxCollider.center = bounds.center - transform.position;
		_boxCollider.size = bounds.size;
	}
}
