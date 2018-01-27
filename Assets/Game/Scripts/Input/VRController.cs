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

    private IDirectable _target;

    [SerializeField]
    private Transform _tip;
    [SerializeField]
    private float _castRedious = 0.02f;


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
    }

    private void OnTriggerPressed()
    {
        RaycastHit raycastHit; 
        if(Physics.SphereCast(_tip.position, _castRedious, _tip.forward, out raycastHit, _castRedious))
        {
            _target = raycastHit.transform.GetComponent<IDirectable>();
            _target.OnSelected();
        }
    }

    private void onTriggerReleased()
    {
        if(_target != null)
        {
            _target.OnDeselected();
            _target = null;
        }
    }



}
