using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRController : MonoBehaviour
{
    private OVRInput.Controller _controllerType;
    private VRInputManager _inputManager;

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
    }

}
