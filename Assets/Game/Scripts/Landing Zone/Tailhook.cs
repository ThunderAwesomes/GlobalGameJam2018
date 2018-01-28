using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Tailhook : MonoBehaviour, IPathable
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

	private void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_landingPath = new Flightpath(transform.position);
		for (int i = 0; i < _pathSegements.Length; i++)
		{
			_landingPath.AddPosition(_pathSegements[i]);
		}
		_landingPath.drawPath = false;
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
		iDirectable.AssignPath(_landingPath);
	}

	/// <summary>
	/// Invoked whenever we have had a plan exit it's path
	/// </summary>
	private void OnExitedPath(Flightpath flightPath, IDirectable IDirectable)
	{
		flightPath.drawPath = flightPath.occupancy > 0;
	}

	private void Update()
	{
		//_currentAlpha = Mathf.Lerp(_currentAlpha, _targetAlpha, Time.deltaTime);
		Color color = Color.white;
		color.a = _currentAlpha;
		_lineRenderer.material.SetColor("_Tint", color);
	}

	private void UpdateLineRenderer()
	{
		if (_lineRenderer.positionCount != _landingPath.count)
		{
			_lineRenderer.positionCount = _landingPath.count;
			_pathSegements = new Vector3[_landingPath.count];
			for (int i = 0; i < _lineRenderer.positionCount; i++)
			{
				_pathSegements[i] = _landingPath[i];
				_lineRenderer.SetPosition(i, _landingPath[i]);
			}
		}
	}


	public void SetPath(Flightpath landingPath)
	{
		_landingPath = landingPath;
	}

	public void OnSelectionStateChanged(SelectionState state)
	{
		Tint.ByState(state, gameObject);
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
}
