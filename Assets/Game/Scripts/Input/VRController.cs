using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRController : MonoBehaviour
{
	public enum SelectionState
	{
		None,
		Hover,
		Select,
	}

	private OVRInput.Controller _controllerType;

	private bool _isTriggerDown;
	private bool _wasTriggerDown;
	private bool _isAwaitingEscape;
	private Vector3 _triggerPressedPosition;

	private IDirectable _selected;
	private IDirectable _hoverTarget;



	private Flightpath _activeFlightPath;
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
		_activeFlightPath = null;
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

		if (_isTriggerDown && _selected != null)
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
			_activeFlightPath.AddPosition(current);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		_hoverTarget = other.GetComponentInParent<IDirectable>();
		if (_hoverTarget != null && !_isTriggerDown)
		{
			_hoverTarget.OnSelectionStateChanged(SelectionState.Hover);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (_hoverTarget != null)
		{
			if (!_isTriggerDown)
			{
				_hoverTarget.OnSelectionStateChanged(SelectionState.None);
			}
			_hoverTarget = null;
		}

		// We keep track of the position we clicked and when we exit the collider we use the current 
		// position. This is used to give us two good points.
		if (_selected != null)
		{
			if (other.transform == _selected.transform && _isAwaitingEscape)
			{
				_isAwaitingEscape = false;
				_activeFlightPath.AddPosition(_triggerPressedPosition);
				_activeFlightPath.AddPosition(_tip.position);
			}
		}
	}

	private void OnTriggerPressed()
	{
		_triggerPressedPosition = _tip.position;
		if (_hoverTarget != null)
		{
			_isAwaitingEscape = true;
			_selected = _hoverTarget;
			_activeFlightPath = new Flightpath(_tip.position);
			_selected.SetFlightpath(_activeFlightPath);
			_selected.OnSelectionStateChanged(SelectionState.Select);
		}
	}

	private void onTriggerReleased()
	{
		if (_selected != null)
		{
			_isAwaitingEscape = false;
			_selected.OnSelectionStateChanged(SelectionState.None);
			_selected = null;
			_activeFlightPath.finalized = true;
			_activeFlightPath = null;

			if (_hoverTarget != null)
			{
				_hoverTarget.OnSelectionStateChanged(SelectionState.Hover);
			}
		}


	}
}
