using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Aeroplane : MonoBehaviour
{
	private Vector3 _targetFacing;
	private Vector3 _targetUp;
	private float _throttle;

	public Vector3 targetFacing
	{
		get { return _targetFacing; }
		set { _targetFacing = value.normalized; }
	}

	public Vector3 targetUp
	{
		get { return _targetUp; }
		set { _targetUp = value.normalized; }
	}

	public float throttle
	{
		get { return _throttle; }
		set { _throttle = Mathf.Clamp(value, 0.0f, 1.0f); }
	}

	[SerializeField]
	protected float _thrustPower = 10000.0f;
	[SerializeField]
	protected float _mass = 500.0f;
	[SerializeField]
	protected Vector3 _axialDragCoefficients = new Vector3(50.0f, 80.0f, 4.0f); // Axial drag coefficient multiplied by area
	[SerializeField]
	protected float _angularDrag = 0.1f;
	[SerializeField]
	protected float _manouverability = 5000.0f;
	[SerializeField]
	protected Vector3 _axialManouverabilityMultiplier = Vector3.one;

	private Rigidbody _rb;

	protected void Start()
	{
		_rb = GetComponent<Rigidbody>();
		_rb.mass = _mass;
		_rb.drag = 0.0f; // We'll calculate the drag forces ourselves.
		_rb.angularDrag = _angularDrag;
		_rb.useGravity = false;
		_rb.WakeUp();
	}

	Vector3 GetDrag()
	{
		float airDensity = 7.0f;

		// You'd think 'oh just multiply the velocity by itself', but you'd be wrong. Then you get negative values going positive and shit hits the fan.
		Vector3 localVelocitySquared = transform.InverseTransformDirection(_rb.velocity.normalized) * _rb.velocity.magnitude * _rb.velocity.magnitude;

		return -0.5f * airDensity * Vector3.Scale(localVelocitySquared, _axialDragCoefficients);
	}

	protected void FixedUpdate()
	{
		// Engine force
		float maxThrustForce = _thrustPower * _mass;
		Vector3 force = transform.forward * (maxThrustForce * throttle);
		_rb.AddForce(force, ForceMode.Force);

		// Manouvering
		Quaternion targetRotation = Quaternion.LookRotation(targetFacing, targetUp);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime * _manouverability); // Hacky but works for now.

		// Drag
		_rb.AddRelativeForce(GetDrag(), ForceMode.Force);
	}
}
