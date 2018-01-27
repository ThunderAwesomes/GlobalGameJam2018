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

	private IDirectable _selected;
	private IDirectable _hoverTarget;



	private Flightpath _activeFlightPath;
	private Transform _anchor;

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

		Vector3 previousPosition = _tip.position;

		transform.position = OVRInput.GetLocalControllerPosition(_controllerType);
		transform.rotation = OVRInput.GetLocalControllerRotation(_controllerType);

		transform.position += _anchor.position;
		transform.rotation *= _anchor.rotation;

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
			UpdateFlightPath(previousPosition, _tip.position);
		}
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
	}

	private void OnTriggerPressed()
	{
		if (_hoverTarget != null)
		{
			_selected = _hoverTarget;
			_activeFlightPath = new Flightpath(_tip.position);
			_selected.flightpath = _activeFlightPath;
			_selected.OnSelectionStateChanged(SelectionState.Select);
		}
	}


	private void onTriggerReleased()
	{
		if (_selected != null)
		{
			_selected.OnSelectionStateChanged(SelectionState.None);
			_selected = null;
			_activeFlightPath = null;

			if (_hoverTarget != null)
			{
				_hoverTarget.OnSelectionStateChanged(SelectionState.Hover);
			}
		}


	}
}
