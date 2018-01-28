using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tailhook))]
public class TailHookEditor : Editor
{
	private SerializedProperty _pathSegements;

	private void OnEnable()
	{
		_pathSegements = serializedObject.FindProperty("_pathSegements");
	}

	public void OnSceneGUI()
	{
		Handles.color = Color.green;
		Tailhook instance = target as Tailhook;
		Vector3 position = instance.transform.position;
		Vector3 previousPosition = Vector3.zero;
		for (int i = 0; i < _pathSegements.arraySize; i++)
		{
			SerializedProperty vectorProperty = _pathSegements.GetArrayElementAtIndex(i);
			Vector3 localPosition = vectorProperty.vector3Value;

			Vector3 worldPosition = localPosition + position;

			EditorGUI.BeginChangeCheck();
			{
				worldPosition = Handles.PositionHandle(worldPosition, Quaternion.identity);
			}
			if (EditorGUI.EndChangeCheck())
			{
				localPosition = worldPosition - position;
				vectorProperty.vector3Value = localPosition;
			}
			int controlID = GUIUtility.GetControlID(FocusType.Passive);
			Handles.SphereHandleCap(controlID, worldPosition, Quaternion.identity, 1f, Event.current.type);
			previousPosition = localPosition;
		}
		serializedObject.ApplyModifiedProperties();
		Handles.color = Color.white;
	}

}
