using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingZone : MonoBehaviour
{
	private class LandingState
	{
		public IDirectable target;
		public float timeout;
		public int gateIndex = -1;
		public bool hitWall;
		public bool isValid = true;

		public void OnEnteredGate(int index)
		{
			if (index != gateIndex + 1)
			{
				isValid = false;
			}
			gateIndex = index;
		}
	}


	[SerializeField]
	private float _pathBreakSize = 0.6f;
	[SerializeField]
	private float _landingTimeout = 2f;

	[SerializeField]
	private LandingWall _leftWall;
	[SerializeField]
	private LandingWall _rightWall;
	[SerializeField]
	private LandingWall _ceiling;
	[SerializeField]
	private float _spacing = 1f;

	[SerializeField]
	private List<LandingNode> _landingNodes;
	[SerializeField]
	private Vector3 _colliderSize = new Vector3(16, 3, 3);

	private List<LandingState> _landingStates = new List<LandingState>();

	// Editor Only
	private void OnValidate()
	{
		float length = _landingNodes.Count * (_spacing + _colliderSize.z);
		for (int i = 0; i < _landingNodes.Count; i++)
		{
			_landingNodes[i].landingZone = this;
			_landingNodes[i].boxCollider.size = Vector3.one;
			_landingNodes[i].transform.localScale = _colliderSize;
			_landingNodes[i].id = i;
			_landingNodes[i].name = "Landing Trigger " + i.ToString();
			_landingNodes[i].transform.localPosition = new Vector3(0, _colliderSize.y / 2f, (_spacing + _colliderSize.z) * i + (_colliderSize.z * 0.5f));
		}
		float halfWidth = _colliderSize.x / 2f;
		float wallHeight = _colliderSize.y + _pathBreakSize;
		float wallCenter = (wallHeight / 2f);


		if (_leftWall != null)
		{
			_leftWall.boxCollider.size = new Vector3(_pathBreakSize, wallHeight, length);
			_leftWall.boxCollider.center = new Vector3(-halfWidth - (_leftWall.boxCollider.size.x * .5f), wallCenter, length / 2f);
			_leftWall.landingZone = this;
		}

		if (_rightWall != null)
		{
			_rightWall.boxCollider.size = new Vector3(_pathBreakSize, wallHeight, length);
			_rightWall.boxCollider.center = new Vector3(halfWidth + (_rightWall.boxCollider.size.x * .5f), wallCenter, length / 2f);
			_rightWall.landingZone = this;
		}

		if (_ceiling != null)
		{
			_ceiling.boxCollider.center = new Vector3(0f, _colliderSize.y + (0.5f * _pathBreakSize), length / 2f);
			_ceiling.boxCollider.size = new Vector3(_colliderSize.x, _pathBreakSize, length);
			_ceiling.landingZone = this;
		}
	}



	private LandingState GetLandingState(IDirectable directable)
	{
		for (int i = 0; i < _landingStates.Count; i++)
		{
			if (_landingStates[i].target == directable)
			{
				if (_landingStates[i].timeout < Time.timeSinceLevelLoad)
				{
					_landingStates.RemoveAt(i);
					break;
				}
				return _landingStates[i];
			}
		}
		LandingState landingState = new LandingState();
		landingState.target = directable;
		landingState.timeout = _landingTimeout + Time.timeSinceLevelLoad;
		_landingStates.Add(landingState);
		return landingState;
	}

	public void OnLandingTunnelHit(LandingWall wall, IDirectable directable)
	{
		GetLandingState(directable).hitWall = true;
	}

	public void OnNodeEntered(LandingNode landingNode, IDirectable directable)
	{
		LandingState state = GetLandingState(directable);
		state.OnEnteredGate(landingNode.id);

		if (landingNode.id == _landingNodes.Count - 1)
		{
			directable.OnLandingAttempted(state.isValid && !state.hitWall, this);
			_landingStates.Remove(state);
		}
	}
}
