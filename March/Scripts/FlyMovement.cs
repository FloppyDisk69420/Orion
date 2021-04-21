using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMovement : MonoBehaviour {
	[Header("Assign")]
	public Rigidbody rb;
	public EntityStats stats;

	[Header("States")]
	public bool sprinting = false;

	[Header("Settings")]
	public float friction = 1f;
	public float checkDist = 0.25f;
	public AnimationCurve accelerationDamp;

	[Space()]
	public bool counterMovement = true;
	public bool restrictMovement = true;

	[Header("Readonly")]
	public Vector3 move = new Vector3();
	
	// other private vars
	float moveSpeed = 0f;
	float maxSpeed = 0f;

	void Awake() {
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		stats = GetComponent<EntityStats>();
	}

	void FixedUpdate() {
		SetMovementVars();		
		Move();

		if (counterMovement) CounterMove();
	}

	void SetMovementVars() {
		if (sprinting) {
			maxSpeed = stats.maxSprintSpeed * stats.agility;
			moveSpeed = stats.sprintSpeed * stats.agility;
		}
		else {
			maxSpeed = stats.maxWalkSpeed * stats.agility;
			moveSpeed = stats.walkSpeed * stats.agility;
		}
	}

	void Move() {
		// apply forces
		float nSpeed = rb.velocity.sqrMagnitude / (maxSpeed * maxSpeed);
		float accelDamp = accelerationDamp.Evaluate(nSpeed);
		float speed = moveSpeed * Time.deltaTime * accelDamp;

		Vector3 xForce = transform.right * move.x * speed;
		Vector3 yForce = transform.up * move.y * speed;
		Vector3 zForce = transform.forward * move.z * speed;

		rb.AddForce(xForce, ForceMode.Acceleration);
		rb.AddForce(yForce, ForceMode.Acceleration);
		rb.AddForce(zForce, ForceMode.Acceleration);
	}

	// stops/slows movement
	void CounterMove() {
		// makes it move slower if the entity is not moving or the entity is faster than the max speed
		float angle = Vector3.Angle(rb.velocity, move);
		if (angle > 0.1f || move.magnitude == 0 || (restrictMovement && rb.velocity.sqrMagnitude > maxSpeed * maxSpeed)) {
			// makes counter move smooth
			float frictionX = rb.velocity.x.Map(-maxSpeed, maxSpeed, -friction, friction);
			float frictionY = rb.velocity.y.Map(-maxSpeed, maxSpeed, -friction, friction);
			float frictionZ = rb.velocity.z.Map(-maxSpeed, maxSpeed, -friction, friction);
			
			float speed = moveSpeed * Time.deltaTime;
			Vector3 force = new Vector3(frictionX, frictionY, frictionZ);
			force *= speed;
			// make force negative so it counteracts the movement
			rb.AddForce(-force);
		}
	}
}
