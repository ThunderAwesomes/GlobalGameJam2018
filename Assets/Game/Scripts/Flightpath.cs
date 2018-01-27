﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aeroplane : MonoBehaviour
{
	private float _yaw;
	private float _pitch;
	private float _roll;
	private float _throttle;

	public float yaw
	{
		get { return _yaw; }
		set { _yaw = Mathf.Clamp(value, -1.0f, 1.0f); }
	}

	public float pitch
	{
		get { return _pitch; }
		set { _pitch = Mathf.Clamp(value, -1.0f, 1.0f); }
	}
	public float roll
	{
		get { return _roll; }
		set { _roll = Mathf.Clamp(value, -1.0f, 1.0f); }
	}
	public float throttle
	{
		get { return _throttle; }
		set { _throttle = Mathf.Clamp(value, 0.0f, 1.0f); }
	}


	[SerializeField]
	protected float thrustPower;
	[SerializeField]
	protected float mass;
	[SerializeField]
	protected float drag;
	[SerializeField]
	protected float manouverability;

	protected void FixedUpdate()
	{

	}
}
