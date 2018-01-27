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
	protected float thrustPower = 5000.0f;
	[SerializeField]
	protected float mass = 1000.0f;
	[SerializeField]
	protected Vector3 axialDragCoefficients = new Vector3(0.5f, 0.8f, 0.05f); // Axial drag coefficient multiplied by area
	[SerializeField]
	protected float angularDrag = 0.1f;
	[SerializeField]
	protected float manouverability = 1000.0f;

	private Rigidbody rb;

	protected void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.mass = mass;
		rb.drag = 0.0f; // We'll calculate the drag forces ourselves.
		rb.angularDrag = angularDrag;
		rb.useGravity = false;
		rb.WakeUp();
	}

	Vector3 GetDrag()
	{
		float airDensity = 7.0f;

		// You'd think 'oh just multiply the velocity by itself', but you'd be wrong. Then you get negative values going positive and shit hits the fan.
		Vector3 localVelocitySquared = transform.InverseTransformDirection(rb.velocity.normalized) * rb.velocity.magnitude * rb.velocity.magnitude;

		return -0.5f * airDensity * Vector3.Scale(localVelocitySquared, axialDragCoefficients);
	}

	protected void FixedUpdate()
	{
		// Engine force
		Vector3 force = transform.forward * (thrustPower * throttle);
		rb.AddForce(force, ForceMode.Force);

		// Steering
		rb.AddRelativeTorque(new Vector3(roll, yaw, pitch) * manouverability, ForceMode.Force);

		// Drag
		rb.AddRelativeForce(GetDrag(), ForceMode.Force);
	}
}
