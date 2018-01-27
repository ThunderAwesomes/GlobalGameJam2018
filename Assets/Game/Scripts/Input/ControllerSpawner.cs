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
		VRController controller = Instantiate<VRController>(_vrControllerPrefab);
		controller.name = name;
		controller.transform.SetParent(transform);
		controller.transform.localPosition = Vector3.zero;
		controller.transform.localScale = Vector3.one;
		controller.AssignController(_type, transform);
	}
}
