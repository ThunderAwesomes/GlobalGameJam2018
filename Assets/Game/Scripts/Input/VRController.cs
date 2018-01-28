using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRController : MonoBehaviour
{
	private OVRInput.Controller _controllerType;

	private bool _isTriggerDown;
	private bool _wasTriggerDown;
	private bool _isAwaitingEscape;
	private Vector3 _triggerPressedPosition;


	private bool _hasPathableSelected;
	private ISelectable _selected;
	private ISelectable _hoverTarget;

	private Transform _anchor;
	private Vector3 _previousPosition;

	[SerializeField]
	private Transform _tip;
	[SerializeField]
	private VRControllerRuntimeSet _runtimeSet;
	[SerializeField]
	private PlayerTransformRuntimeSet _playerTransformRuntimeSet;

	private float _nextWaypoint = 0f;


	private void OnDestroy()
	{
		OnControllerDisconnected();
	}

	private void OnEnable()
	{
		_runtimeSet.Add(this);
	}

	private void OnDisable()
	{
		_runtimeSet.Remove(this);
	}

	public void AssignController(OVRInput.Controller type, Transform anchor)
	{
		_controllerType = type;
		_anchor = anchor;
	}

	public void OnControllerConnected()
	{
	}

	public void OnControllerDisconnected()
	{
		_selected = null;
	}

	private void Update()
	{
		_isTriggerDown = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, _controllerType);

		if (_wasTriggerDown != _isTriggerDown)
		{
			if (_isTriggerDown)
			{
				OnTriggerPressed();
			}
			else
			{
				onTriggerReleased();
			}
		}
		_wasTriggerDown = _isTriggerDown;

		if (_isTriggerDown && _hasPathableSelected)
		{
			UpdateFlightPath(_previousPosition, _tip.position);
		}
		_previousPosition = _tip.position;
	}

	private void UpdateFlightPath(Vector3 previous, Vector3 current)
	{
		Vector3 delta = previous - current;
		float magnitude = Vector3.Magnitude(delta) * _playerTransformRuntimeSet.scale;
		_nextWaypoint -= magnitude;

		if (_nextWaypoint <= 0)
		{
			_nextWaypoint = _runtimeSet.waypointDelta;
			IPathable pathable = _selected as IPathable;
			if (!pathable.IsNull())
			{
				pathable.AddPathPosition(current);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		_hoverTarget = other.GetComponentInParent<ISelectable>();
		if (_hoverTarget.isInteractable)
		{
			if (!_hoverTarget.IsNull() && !_isTriggerDown)
			{
				_hoverTarget.OnSelectionStateChanged(SelectionState.Highlighted);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		ISelectable otherSelection = other.GetComponent<ISelectable>();
		if (otherSelection.isInteractable)
		{
			if (!_hoverTarget.IsNull())
			{
				if (!_isTriggerDown)
				{
					_hoverTarget.OnSelectionStateChanged(SelectionState.None);
				}
				_hoverTarget = null;
			}

			// We keep track of the position we clicked and when we exit the collider we use the current 
			// position. This is used to give us two good points.
			if (!_selected.IsNull())
			{
				if (!otherSelection.IsNull() && otherSelection == _selected)
				{
					_isAwaitingEscape = false;
					IPathable pathable = _selected as IPathable;
					if (!pathable.IsNull())
					{
						pathable.AddPathPosition(_triggerPressedPosition);
						pathable.AddPathPosition(_tip.position);
					}
				}
			}
		}
	}

	private void OnTriggerPressed()
	{
		_triggerPressedPosition = _tip.position;
		if (!_hoverTarget.IsNull())
		{
			_isAwaitingEscape = true;
			_selected = _hoverTarget;

			IPathable pathable = _selected as IPathable;
			_hasPathableSelected = !pathable.IsNull();

			// Only pathable objects can have paths 
			if (_hasPathableSelected)
			{
				pathable.StartPath(_tip.position);
			}

			_selected.OnSelectionStateChanged(SelectionState.Pressed);
		}
	}

	private void onTriggerReleased()
	{
		if (!_selected.IsNull())
		{
			_isAwaitingEscape = false;
			_selected.OnSelectionStateChanged(SelectionState.None);
			_selected = null;
			IPathable pathable = _selected as IPathable;
			if (!pathable.IsNull())
			{
				pathable.EndPath();
			}

			if (_hoverTarget != null)
			{
				_hoverTarget.OnSelectionStateChanged(SelectionState.Highlighted);
			}
		}


	}
}
