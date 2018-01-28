using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathable : ISelectable
{
	void StartPath(Vector3 position);
	void AddPathPosition(Vector3 position);
	void EndPath();
}
