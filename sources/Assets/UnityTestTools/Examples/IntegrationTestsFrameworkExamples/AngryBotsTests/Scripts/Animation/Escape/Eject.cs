using UnityEngine;
using System.Collections;


[RequireComponent (typeof (Rigidbody))]
public class Eject : MonoBehaviour
{
	public Vector3 force, torque;


	void Start ()
	{
		rigidbody.useGravity = true;
		rigidbody.AddForce (force);
		rigidbody.AddTorque (torque);
	}
	
	
	void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine (transform.position, transform.position + force.normalized);
	}
}
