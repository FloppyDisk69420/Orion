using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickupController : MonoBehaviour {
	Rigidbody rb;
	
	public float speed = 40f;
	public float rotSpeed = 40f;
	public float smoothSpeed = 2f;
	public float gravity = 3f;
	public float maxSpeed = 12f;
	public float counterForce = 4f;
	public float groundCheck = 0.3f;
	public Vector3 rotBase;
	public Vector3 newRot;
	public bool rotate = true;

	bool grounded = false;

	void Awake() {
		rb = GetComponent<Rigidbody>();
		// we are going to make our own gravity
		rb.useGravity = false;
		rotBase = GetRandomRotation();
		newRot = GetRandomRotation();
		grounded = false;
	}
	
	Vector3 GetRandomRotation() {
		return Random.rotation.eulerAngles.normalized;
	}

	void Update() {
		if (rotate) {
			newRot = GetRandomRotation();
			// add rotation
			transform.rotation *= Quaternion.Euler(rotBase * rotSpeed * Time.deltaTime);
			// smooth the random
			Quaternion newQ = Quaternion.Euler(newRot);
			Quaternion baseQ = Quaternion.Euler(rotBase);
			rotBase = Quaternion.Slerp(baseQ, newQ, Time.deltaTime * smoothSpeed).eulerAngles.normalized;
		}
	}

	void FixedUpdate() {
		RaycastHit hit;
		grounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheck);

		if (grounded || rb.velocity.y > maxSpeed) {
			// get vertical distance
			float offset = Mathf.Abs(transform.position.y - hit.point.y);
			offset -= groundCheck;
			// counteract current gravity
			float m = rb.velocity.y.Map(-maxSpeed, maxSpeed, -counterForce, counterForce);
			//               counter gravity       go just above ground
			rb.AddForce(Vector3.down * m * speed + Vector3.down * offset);
		}

		if (!grounded) {
			rb.AddForce(Vector3.down * gravity);
		}
		else {
			float xM = rb.velocity.x.Map(-maxSpeed, maxSpeed, -counterForce, counterForce);
			float zM = rb.velocity.z.Map(-maxSpeed, maxSpeed, -counterForce, counterForce);

			Vector3 counter = new Vector3(xM, 0, zM);
			rb.AddForce(-counter * speed);
		}
	}

	public void Delete() {
		Destroy(gameObject);
	}
}
