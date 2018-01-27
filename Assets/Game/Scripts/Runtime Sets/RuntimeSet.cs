using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public abstract class RuntimeSet<T> : BaseRuntimeSet
{
	[SerializeField, HideInInspector]
	private List<T> _instances;

	/// <summary>
	/// Returns back the count for the number of instances we have.
	/// </summary>
	public int count
	{
		get { return _instances.Count; }
	}

	/// <summary>
	/// Returns back the type of set this element is
	/// </summary>
	public override Type setType
	{
		get
		{
			return typeof(T);
		}
	}

	public RuntimeSet()
	{
		_instances = new List<T>();
	}

	/// <summary>
	/// Invoked when we first access this runtime set. Always call base
	/// </summary>
	protected virtual void OnEnable()
	{
#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
#endif
	}

	/// <summary>
	/// Invoked when we exit play mode or this object is destroyed. Always call base
	/// </summary>
	protected virtual void OnDisable()
	{
#if UNITY_EDITOR
		EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
#endif
	}

#if UNITY_EDITOR
	/// <summary>
	/// Invoked whenever the play mode state changes. Used to clear our runtime sets. 
	/// </summary>
	/// <param name="playmodeState"></param>
	private void OnPlaymodeStateChanged(PlayModeStateChange playmodeState)
	{
		switch (playmodeState)
		{
			case PlayModeStateChange.EnteredEditMode:
			case PlayModeStateChange.ExitingPlayMode:
				_instances.Clear();
				break;
		}
	}
#endif

	/// <summary>
	/// Returns back a readonly collection for the instances 
	/// </summary>
	public ReadOnlyCollection<T> instances
	{
		get { return _instances.AsReadOnly(); }
	}

	/// <summary>
	/// Adds an instance to this runtime set. 
	/// </summary>
	public void Add(T instance)
	{
		_instances.Add(instance);
	}

	/// <summary>
	/// Removes an instance from this runtime set. 
	/// </summary>
	/// <returns>Returns true if it existed and false if it did not</returns>
	public bool Remove(T instance)
	{
		return _instances.Remove(instance);
	}
}
