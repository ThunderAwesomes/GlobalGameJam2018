using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LandingWall : MonoBehaviour
{
	private BoxCollider _collider;

	[SerializeField]
	private LandingZone _landingZone;


	public LandingZone landingZone
	{
		get { return _landingZone; }
		set { _landingZone = value; }
	}

	public BoxCollider boxCollider
	{
		get
		{
			if (_collider == null)
			{
				_collider = GetComponent<BoxCollider>();
			}
			return _collider;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		IDirectable iDirectable = other.GetComponent<IDirectable>();

		if (iDirectable != null)
		{
			_landingZone.OnLandingTunnelHit(this, iDirectable);
		}
	}
}
