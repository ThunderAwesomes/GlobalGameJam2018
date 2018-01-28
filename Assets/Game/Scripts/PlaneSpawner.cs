using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PlaneSpawner : MonoBehaviour
{
	[SerializeField]
	private PlaneFactory _factory;
	[SerializeField]
	private PlayerTransformRuntimeSet _playerTransform;

	[SerializeField]
	private float _spawnRate = 2;
	[SerializeField]
	private float _radius = 10f;
	[SerializeField]
	private float _primaryTargetRadiusOffset = 1f;
	[SerializeField]
	private float _primaryTargetHeightOffset = 1f;
	[SerializeField]
	private Transform _root;

	[Header("Player Position")]
	private float _holdingPositionRadius = 2f;
	private float _holdingPositionMin = 1f;
	private float _holdingPoistionMax = 2;


	private float _spawnTime = 3f;

	public void Update()
	{
		if (Application.isPlaying)
		{
			_spawnTime -= Time.deltaTime;
			if (_spawnTime <= 0f)
			{
				_spawnTime = _spawnRate;
				Spawn();
			}
		}
	}

	private void OnEnable()
	{
#if UNITY_EDITOR
		SceneView.onSceneGUIDelegate += OnDrawHandles;
#endif
	}

#if UNITY_EDITOR
	private void OnDrawHandles(SceneView sceneView)
	{
		if (_root != null)
		{
			Handles.color = Color.green;
			Handles.DrawWireDisc(_root.position, Vector3.up, _radius);
			Vector3 heightOffset = _root.position;
			heightOffset.y += _primaryTargetHeightOffset;
			Handles.DrawWireDisc(heightOffset, Vector3.up, _radius - _primaryTargetRadiusOffset);
			Handles.color = Color.white;
		}
	}
#endif

	private void OnDisable()
	{
#if UNITY_EDITOR
		SceneView.onSceneGUIDelegate -= OnDrawHandles;
#endif
	}

	private void Spawn()
	{
		float angle = ((Mathf.PI * 2) * Random.value);
		float sin = Mathf.Sin(angle);
		float cos = Mathf.Cos(angle);

		// Pick a spawn position
		Vector3 spawnPosition = _root.position;
		spawnPosition.x += _radius * sin;
		spawnPosition.z += _radius * cos;
		GameObject go = _factory.CreateRandomPlane(spawnPosition, Quaternion.identity);
		go.transform.SetParent(_root);

		// Grab our directable component
		IDirectable iDirectable = go.GetComponent<IDirectable>();
		// Set our primary travel position 
		Vector3 primaryTravelPosition = _root.position;
		primaryTravelPosition.x += (_radius - _primaryTargetRadiusOffset) * sin;
		primaryTravelPosition.z += (_radius - _primaryTargetRadiusOffset) * cos;
		primaryTravelPosition.y += _primaryTargetHeightOffset;

		// Secondary Point
		angle = ((Mathf.PI * 2) * Random.value);
		Vector3 holdingPosition = _playerTransform.position;
		float hpRadius = Random.value * _holdingPositionRadius * _playerTransform.scale;
		holdingPosition.x += sin * hpRadius;
		holdingPosition.z += cos * hpRadius;

		Flightpath flightPath = new Flightpath(primaryTravelPosition);
        flightPath.drawPath = false;
        flightPath.AddPosition(holdingPosition);
		flightPath.Finialized();
		iDirectable.AssignPath(flightPath);
	}
}
