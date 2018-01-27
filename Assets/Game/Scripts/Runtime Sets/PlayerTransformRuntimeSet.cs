using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable, CreateAssetMenu(menuName = "Runtime Sets/Player Transform")]
public class PlayerTransformRuntimeSet : RuntimeSet<PlayerTransform>
{
	[SerializeField]
	private float _startingHeight;
	[SerializeField]
	private float _startingScale;
	[SerializeField]
	private Vector3 _startingPosition;

	private float _height;
	private float _scale;
	private Vector3 _position;

	/// <summary>
	/// Returns the height of the player.
	/// </summary>
	public float height
	{
		get { return _height; }
	}

	/// <summary>
	/// Returns the scale of the player
	/// </summary>
	public float scale
	{
		get { return _startingScale; }
	}

	/// <summary>
	/// Returns the position of the player
	/// </summary>
	public Vector3 position
	{
		get { return _startingPosition; }
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		_height = _startingHeight;
		_scale = _startingScale;
		_position = _startingPosition;
	}
}
