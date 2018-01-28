using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable, CreateAssetMenu(menuName = "Runtime Sets/Player Transform")]
public class PlayerTransformRuntimeSet : RuntimeSet<PlayerTransform>
{
	[SerializeField,Range(1f, 200f)]
	private float _startingScale = 10f;
	[SerializeField]
	private Vector3 _startingPosition = Vector3.zero;

	private float _playerHeight;
	private float _scale;
	private Vector3 _position;

	/// <summary>
	/// Returns the height of the player.
	/// </summary>
	public float playerHeight
	{
		get { return _playerHeight; }
		set { _playerHeight = value; }
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
		_scale = _startingScale;
		_position = _startingPosition;
	}
}
