using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerTransformRuntimeSet))]
public class PlayerTransformRuntimeSetEditor : BaseRuntimeSetEditor
{
	private static readonly string[] IGNORED_FIELDS = new[]
	{ "_instances",
		"m_Script",
		"_startingScale",
		"_startingPosition"
	};

	private SerializedProperty _startingScale;
	private SerializedProperty _startingPosition;


	public override string[] ignoredFields
	{
		get
		{
			return IGNORED_FIELDS;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		_startingScale = serializedObject.FindProperty("_startingScale");
		_startingPosition = serializedObject.FindProperty("_startingPosition");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorGUI.BeginChangeCheck();
		{
			EditorGUILayout.PropertyField(_startingScale);
			EditorGUILayout.PropertyField(_startingPosition);
		}
		if (EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
			PlayerTransformRuntimeSet runtimeSet = (PlayerTransformRuntimeSet)target;
			foreach(PlayerTransform transform in runtimeSet.instances)
			{
				transform.Reset();
			}
		}
	}
}
