using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDirectable 
{
	GameObject gameObject { get; }
	Transform transform { get; }
	Flightpath flightpath { get; set; }

	void OnSelectionStateChanged(VRController.SelectionState state);
}
