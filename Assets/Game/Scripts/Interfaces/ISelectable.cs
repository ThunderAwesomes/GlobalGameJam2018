using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionState
{
	None,
	Pressed,
	Released,
	Highlighted,
}

public interface ISelectable : IUnityObject
{
	bool isInteractable { get; set; }
	void OnSelectionStateChanged(SelectionState newState);
}
