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

    private void onTriggerReleased()
    {
        Debug.Log("Released : " + _controllerType);
    }

    private void OnTriggerPressed()
    {
        Debug.Log("Pressed : " + _controllerType);
    }
}
