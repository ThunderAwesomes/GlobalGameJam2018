using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LandingNode : MonoBehaviour
{
	[SerializeField]
	private int _id;

	private BoxCollider _collider;

	public BoxCollider boxCollider
	{
		get
		{
			if(_collider == null)
			{
				_collider = GetComponent<BoxCollider>();
			}
			return _collider;
		}
	}

	
	public int id
	{
		get { return _id; }
		set { _id = value; }
	}
}
