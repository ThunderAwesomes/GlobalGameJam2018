using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDirectable 
{
	Flightpath flightpath { get; set; }

	void OnSelectionStateChanged(VRController.SelectionState state);
}
