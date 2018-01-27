using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRController : MonoBehaviour
{
    private OVRInput.Controller _controllerType;
    private VRInputManager _inputManager;

    private bool _isTriggerDown;
    private bool _wasTriggerDown;
    private bool _hasTarget;

    private IDirectable _target;
    private Flightpath _activeFlightPath;

    [SerializeField]
    private Transform _tip;
    [SerializeField]
    private float _castRadius = 0.02f;

    private void OnDestroy()
    {
        OnControllerDisconnected();
    }

    public void AssignController(VRInputManager inputManager, OVRInput.Controller type)
    {
        _inputManager = inputManager;
        _controllerType = type;
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

    private void Update()
    {
        transform.position = OVRInput.GetLocalControllerPosition(_controllerType);
        transform.rotation = OVRInput.GetLocalControllerRotation(_controllerType);

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

        if(_hasTarget)
        {
            UpdateTarget();
        }
    }



    private void OnTriggerPressed()
    {
        RaycastHit raycastHit;
        Ray ray = new Ray(_tip.position, _tip.forward);

        if (Physics.SphereCast(ray, _castRadius,out raycastHit, 5, Layers.Directable)) 
        {
            _target = raycastHit.transform.GetComponent<IDirectable>();
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
        }
    }



}
