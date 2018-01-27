using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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

	private Rigidbody rb;

	protected void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.mass = mass;
		rb.drag = drag;
		rb.WakeUp();
	}

	protected void FixedUpdate()
	{
		// Engine force
		Vector3 force = transform.forward * (thrustPower * throttle);
		rb.AddForce(force, ForceMode.Force);

		// Temp until we have some lift and hsit.
		rb.useGravity = false;
	}
}
