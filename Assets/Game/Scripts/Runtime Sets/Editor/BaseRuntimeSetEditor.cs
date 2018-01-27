using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseRuntimeSet), editorForChildClasses:true)]
public class BaseRuntimeSetEditor : Editor
{

	private static readonly string[] INSTANCES_FIELD_NAME = new[] { "_instances", "m_Script" };
	private SerializedProperty _instances;

	private void OnEnable()
	{
		_instances = serializedObject.FindProperty(INSTANCES_FIELD_NAME[0]);
	}

	public override void OnInspectorGUI()
	{
		_instances.isExpanded = EditorGUILayout.Foldout(_instances.isExpanded, _instances.displayName);
		if (_instances.isExpanded)
		{
			EditorGUI.indentLevel++;
			if (!serializedObject.isEditingMultipleObjects)
			{
				for (int i = 0; i < _instances.arraySize; i++)
				{
					SerializedProperty element = _instances.GetArrayElementAtIndex(i);
					Object instanceValue = element.objectReferenceValue;
					BaseRuntimeSet runtimeSet = (BaseRuntimeSet)target;
					EditorGUILayout.ObjectField(i.ToString(), instanceValue, runtimeSet.setType, true);
				}
			}
			EditorGUI.indentLevel--;
		}
		Editor.DrawPropertiesExcluding(serializedObject, INSTANCES_FIELD_NAME);
	}
}
