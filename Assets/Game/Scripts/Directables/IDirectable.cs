using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface IDirectable : IPathable
{
	Flightpath flightpath { get; }
	T GetComponent<T>();
	void AssignPath(Flightpath flightpath);
}
