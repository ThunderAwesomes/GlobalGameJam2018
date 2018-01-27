using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class VRInputManager : MonoBehaviour
{
    // Prefabs
    [SerializeField]
    private VRController _vrControllerPrefab;

    // Instances
    private VRController _leftTrackedController;
    private VRController _rightTrackedController;

    private void Start()
    {
        CreateControllers();
    }


    private void CreateControllers()
    {
        Assert.IsNull(_leftTrackedController, "Left controller expected to be null");
        Assert.IsNull(_rightTrackedController, "Right controller expected to be null");
        _leftTrackedController = CreateController(OVRInput.Controller.LTouch);
        _rightTrackedController = CreateController(OVRInput.Controller.RTouch);
    }

    private VRController CreateController(OVRInput.Controller controllerType)
    {
        string name = _vrControllerPrefab.name + " [" + controllerType.ToString() + "]";
        VRController controller = Instantiate<VRController>(_vrControllerPrefab);
        controller.name = name;
        controller.transform.SetParent(transform);
        controller.transform.localPosition = Vector3.zero;
        controller.AssignController(this, controllerType);
        return controller;
    }
}
