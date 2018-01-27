using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDirectable 
{
	Flightpath flightpath { get; set; }

	void OnHover();
	void OnSelected();
	void OnDeselected();
}
