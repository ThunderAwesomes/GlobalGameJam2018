using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingZone : MonoBehaviour
{
	[SerializeField]
	private float _pathBreakSize = 0.6f;

	[SerializeField]
	private BoxCollider _leftWall;
	[SerializeField]
	private BoxCollider _rightWall;
	[SerializeField]
	private BoxCollider _ceiling;
	[SerializeField]
	private float _spacing = 1f;
	
	[SerializeField]
	private List<LandingNode> _landingNodes;
	[SerializeField]
	private Vector3 _colliderSize = new Vector3(16, 3, 3);

	private void OnValidate()
	{
		float length = _landingNodes.Count * (_spacing + _colliderSize.z);
		for (int i = 0; i < _landingNodes.Count; i++)
		{
			_landingNodes[i].boxCollider.size = _colliderSize;
			_landingNodes[i].boxCollider.center = new Vector3(0, _colliderSize.y / 2f, 0);
			_landingNodes[i].id = i;
			_landingNodes[i].name = "Landing Trigger " + i.ToString();
			_landingNodes[i].transform.localPosition = new Vector3(0, 0, (_spacing + _colliderSize.z) * i + (_colliderSize.z * 0.5f));
		}
		float halfWidth = _colliderSize.x / 2f;
		float wallHeight = _colliderSize.y + _pathBreakSize;
		float wallCenter = (wallHeight / 2f);


		if (_leftWall != null)
		{
			_leftWall.size = new Vector3(_pathBreakSize, wallHeight, length);
			_leftWall.center = new Vector3(-halfWidth - (_leftWall.size.x * .5f), wallCenter, length / 2f);
		}

		if (_rightWall != null)
		{
			_rightWall.size = new Vector3(_pathBreakSize, wallHeight, length);
			_rightWall.center = new Vector3(halfWidth + (_rightWall.size.x * .5f), wallCenter, length / 2f);
		}

		if(_ceiling != null)
		{
			_ceiling.center = new Vector3(0f, _colliderSize.y + (0.5f * _pathBreakSize), length / 2f);
			_ceiling.size = new Vector3(_colliderSize.x, _pathBreakSize, length);
		}
	}
}
