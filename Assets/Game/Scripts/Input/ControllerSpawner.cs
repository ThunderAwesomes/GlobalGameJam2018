using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ControllerSpawner : MonoBehaviour
{
    // Prefabs
    [SerializeField]
    private VRController _vrControllerPrefab;
	[SerializeField]
	private OVRInput.Controller _type;

    // Instances
    private VRController _trackedController;

    private void Start()
    {
        CreateControllers();
    }

    private void CreateControllers()
    {
        Assert.IsNull(_trackedController, "Controller expected to be null");
		string name = _vrControllerPrefab.name + " [" + _type.ToString() + "]";
		_trackedController = Instantiate<VRController>(_vrControllerPrefab);
		_trackedController.name = name;
		_trackedController.transform.SetParent(transform);
		_trackedController.transform.localPosition = Vector3.zero;
		_trackedController.transform.localScale = Vector3.one;
		_trackedController.AssignController(_type, transform);
	}
}
