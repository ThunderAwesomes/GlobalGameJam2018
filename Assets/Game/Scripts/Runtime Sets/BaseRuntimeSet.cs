using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRuntimeSet : ScriptableObject
{
	/// <summary>
	/// Returns the type of elements this set contains.
	/// </summary>
	public abstract Type setType
	{
		get;
	}
}
