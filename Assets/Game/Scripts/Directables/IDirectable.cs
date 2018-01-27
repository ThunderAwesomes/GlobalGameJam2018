using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDirectable 
{
    void OnSelected();
	Flightpath flightpath { get; set; }
	void OnDeselected();
}
