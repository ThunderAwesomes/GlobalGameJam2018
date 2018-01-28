using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	public Vector3 axis;
	public float speed;

	void Update ()
	{
		transform.Rotate(axis, Time.deltaTime * speed, Space.Self);
	}
}
