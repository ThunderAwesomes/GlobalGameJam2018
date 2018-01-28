using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Aeroplane : MonoBehaviour
{
	private Vector3 _targetFacing = Vector3.zero;
	private Vector3 _targetUp = Vector3.up;
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
	protected float turnRate = 1.0f;
	[SerializeField]
	protected Vector3 _axialManouverabilityMultiplier = Vector3.one;
	[SerializeField]
	private GameObject _explosionEffect;

	private Rigidbody _rb;

	Vector3 angularAcceleration;

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

	Vector3 CalculateSpringTorque(Vector3 currentVector, Vector3 targetVector, float springConstant)
	{
		Vector3 differenceVector = Vector3.Cross(currentVector, targetVector.normalized);
		float torqueValue = Vector3.Angle(currentVector, targetVector) * springConstant;
		Vector3 deltaAngularVelocity = differenceVector.normalized * torqueValue;
		Quaternion transformQuaternion = transform.rotation * _rb.inertiaTensorRotation;
		return transformQuaternion * Vector3.Scale(_rb.inertiaTensor, (Quaternion.Inverse(transformQuaternion) * deltaAngularVelocity));
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (_explosionEffect != null)
		{
			GameObject explosion = Instantiate(_explosionEffect);
			explosion.transform.position = transform.position;
			Destroy(explosion, 1.0f);
			if (Game.Instance != null)
			{
				Game.Instance.Mode.OnPlaneDestroyed();
			}
		}
		Destroy(gameObject);
	}

	protected void FixedUpdate()
	{
		// Engine force
		float maxThrustForce = _thrustPower * _mass;
		Vector3 force = transform.forward * (maxThrustForce * throttle);
		_rb.AddForce(force, ForceMode.Force);

		// Manouvering
		if (targetFacing != Vector3.zero)
		{
			Vector3 springTargetFacing = Vector3.Slerp(transform.forward, targetFacing, turnRate * Time.fixedDeltaTime);

			//transform.rotation = Quaternion.LookRotation(springTargetFacing);

			_rb.AddTorque(CalculateSpringTorque(transform.forward, springTargetFacing, 1.5f));
			_rb.AddTorque(CalculateSpringTorque(transform.up, targetUp, 0.4f));
		}

		// Drag
		Vector3 relativeDrag = GetDrag();
		_rb.AddRelativeForce(relativeDrag, ForceMode.Force);
	}
}
