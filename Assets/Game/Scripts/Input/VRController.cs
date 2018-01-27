using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRController : MonoBehaviour
{
    private OVRInput.Controller _controllerType;

    private bool _isTriggerDown;
    private bool _wasTriggerDown;
    private bool _hasTarget;

    private IDirectable _target;
    private Flightpath _activeFlightPath;
	private Transform _anchor;

    [SerializeField]
    private Transform _tip;
	[SerializeField]
	private VRControllerRuntimeSet _runtimeSet;

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
        if (_target != null)
        {
            _target.OnDeselected();
            _target = null;
        }
        _activeFlightPath = null;
    }

	private void OnDrawGizmos()
	{
		if (_runtimeSet.isDebug)
		{
			Gizmos.DrawWireSphere(_tip.position, _runtimeSet.sphereCastRadius);
		}
	}

	private void Update()
    {

		Vector3 previousPosition = transform.position;

        transform.position = OVRInput.GetLocalControllerPosition(_controllerType);
		transform.rotation = OVRInput.GetLocalControllerRotation(_controllerType);

		transform.position += _anchor.position;
		transform.rotation *= _anchor.rotation;

        _isTriggerDown = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, _controllerType);

        if(_wasTriggerDown != _isTriggerDown)
        {
            if(_isTriggerDown)
            {
                OnTriggerPressed();
            }
            else
            {
                onTriggerReleased();
            }
        }
        _wasTriggerDown = _isTriggerDown;

        if(_isTriggerDown && _hasTarget)
        {
            UpdateFlightPath(previousPosition, transform.position);
        }
    }

	private void OnPostRender()
	{
		if (_hasTarget)
		{
			_activeFlightPath.DrawDebug();
		}
	}

	private void UpdateFlightPath(Vector3 previous, Vector3 current)
	{
		Vector3 delta = previous - current;
		float magnitude = Vector3.Magnitude(delta);
		_nextWaypoint -= magnitude;

		if(_nextWaypoint <= 0)
		{
			_nextWaypoint = _runtimeSet.waypointDelta;
			_activeFlightPath.AddPosition(current);
		}
	}

    private void OnTriggerPressed()
    {
        RaycastHit raycastHit;
        Ray ray = new Ray(_tip.position, _tip.forward);

        if (Physics.SphereCast(ray, 0f, out raycastHit, _runtimeSet.sphereCastRadius, Layers.Directable)) 
        {
            _target = raycastHit.transform.GetComponent<IDirectable>();
			_activeFlightPath = new Flightpath(_tip.position);
			_target.flightpath = _activeFlightPath;
            _target.OnSelected();
            _hasTarget = true;
		}
        else
        {
            _hasTarget = false;
        }
    }

    private void onTriggerReleased()
    {
        if(_target != null)
        {
            _target.OnDeselected();
            _target = null;
            _hasTarget = false;
			_activeFlightPath = null;

		}
    }



}
