using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface IDirectable 
{

	GameObject gameObject { get; }
	Transform transform { get; }
	Flightpath flightpath { get; }
	void SetFlightpath(Flightpath flightpath);

	void OnSelectionStateChanged(VRController.SelectionState state);
	void OnLandingAttempted(bool wasSuccessful, LandingZone landingZone);
}
