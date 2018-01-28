using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LandingNode : MonoBehaviour
{
	[SerializeField]
	private int _id;
	[SerializeField]
	private LandingZone _landingZone;

	private BoxCollider _collider;


	public LandingZone landingZone
	{
		get { return _landingZone; }
		set { _landingZone = value; }
	}
	

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

	private void OnTriggerEnter(Collider other)
	{
		IDirectable iDirectable = other.GetComponent<IDirectable>();

		if (iDirectable != null)
		{
			_landingZone.OnNodeEntered(this, iDirectable);
		}
	}
}
