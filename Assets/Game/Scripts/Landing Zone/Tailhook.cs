﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Tailhook : MonoBehaviour//, //IPathable
{
	[SerializeField]
	private Vector3[] _pathSegements;
	private Flightpath _landingPath;
	private LineRenderer _lineRenderer;

	private int _activeCount = 0;
	private bool _isDirty;
	private float _currentAlpha = 1f;
	private float _targetAlpha = 1f;

	/// <summary>
	/// Returns back the number of active directables
	/// that are currently using this tail hook. 
	/// </summary>
	public int activeCount
	{
		get { return _activeCount; }
	}

	public Flightpath flightpath
	{
		get
		{
			return _landingPath;
		}
	}

	public LineRenderer lineRenderer
	{
		get
		{
			if (_lineRenderer == null)
			{
				_lineRenderer = GetComponent<LineRenderer>();
				_lineRenderer.useWorldSpace = false;
			}
			return _lineRenderer;
		}
	}

	private void Awake()
	{
		_landingPath = new Flightpath(transform.position);
		_landingPath.lookAheadDistance = 8f;
		for (int i = 0; i < _pathSegements.Length; i++)
		{
			_landingPath.AddPosition(_pathSegements[i] + transform.position);
		}
		_landingPath.drawPath = true;
		_landingPath.disposeOnComplete = false;
		_landingPath.onPathExited += OnExitedPath;
		_landingPath.Finialized();
	}

	/// <summary>
	/// Takes a directable and forces it to land along
	/// the given path. 
	/// </summary>
	/// <param name="iDirectable">The thing you want to land</param>
	public void LandDirectable(IDirectable iDirectable)
	{
		iDirectable.AssignPath(_landingPath, true);
		iDirectable.isInteractable = false;
		//_landingPath.drawPath = _landingPath.occupancy > 0;
	}

	/// <summary>
	/// Invoked whenever we have had a plan exit it's path
	/// </summary>
	private void OnExitedPath(Flightpath flightPath, IDirectable iDirectable)
	{
		flightPath.drawPath = flightPath.occupancy > 0;

		Rigidbody dRigidBody = iDirectable.GetComponent<Rigidbody>();
		dRigidBody.constraints = RigidbodyConstraints.FreezeAll;
		Destroy((Object)iDirectable);
	}

	private void Update()
	{
		//_currentAlpha = Mathf.Lerp(_currentAlpha, _targetAlpha, Time.deltaTime);
		Color color = Color.white;
		color.a = _currentAlpha;
		//_lineRenderer.material.SetColor("_Tint", color);
	}

	private void UpdateLineRenderer()
	{
		lineRenderer.positionCount = _pathSegements.Length;
		for (int i = 0; i < lineRenderer.positionCount; i++)
		{
			lineRenderer.SetPosition(i, _pathSegements[i]);
		}
	}


	public void SetPath(Flightpath landingPath)
	{
		_landingPath = landingPath;
	}

	public void OnSelectionStateChanged(SelectionState state)
	{
		Tint.ByState(state, gameObject);
		if (state == SelectionState.Pressed)
		{
			//_landingPath.drawPath = true;
		}
	}

	public void StartPath(Vector3 position)
	{
		_landingPath.Reset();
		AddPathPosition(position);
	}

	public void AddPathPosition(Vector3 position)
	{
		_landingPath.AddPosition(position);
		UpdateLineRenderer();
	}

	public void EndPath()
	{
		_landingPath.Finialized();
	}

	private void OnValidate()
	{
		UpdateLineRenderer();
	}
}
