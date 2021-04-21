using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour {
	[Header("Assign")]
	public Rigidbody rb;
	public EntityStats stats;
	public Transform feet;

	[Header("States")]
	public bool onGround = false;
	public bool sprinting = false;
	public bool jumping = false;
	public bool crouching = false;
	public bool ableToCrouch = false;
	public bool ableToJump = false;
	public bool wallRiding = false;

	[Header("Settings")]
	public float friction = 1f;
	public float checkDist = 0.25f;
	public float slopeForce = 3f;
	[Range(0, 1)] public float airControl = 0.5f;
	public bool updateNormals = true;
	public bool alwaysDoFriction = false;
	public bool counterMovement = true;

	[Header("Readonly")]
	public Vector3 groundNormal;
	public Vector3 moveForward;
	public Vector3 moveRight;
	public Vector2 move = new Vector2();
	
	// other private vars
	float moveSpeed = 0f;
	float maxSpeed = 0f;
	float aeiralMult = 1f;
	float bottomCSOrigin;
	Dictionary<int, ContactPoint[]> contacts;

	void Awake() {
		rb = GetComponent<Rigidbody>();
		stats = GetComponent<EntityStats>();
		contacts = new Dictionary<int, ContactPoint[]>();
		CapsuleCollider cs = GetComponent<CapsuleCollider>();
		bottomCSOrigin = cs.height / 2 - cs.radius;
		updateNormals = true;
		alwaysDoFriction = false;
		counterMovement = true;
	}

	void FixedUpdate() {
		SetMovementVars();
		UpdateGroundNormal();
		DoJump();
		
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
		// this is used to make the movement looser in the air
		// 		- if alwaysDoFriction is on then the movement is always counteracted
		if (!onGround && !alwaysDoFriction)
			aeiralMult = airControl;
		else
			aeiralMult = 1f;
	}

	void Move() {		
		// cancel input to prevent entity from moving too fast
		Vector3 relVel = FindRelativeVelocity();
		if (relVel.z > moveSpeed || relVel.z < -moveSpeed) move.y = 0;
		if (relVel.x > moveSpeed || relVel.x < -moveSpeed) move.x = 0;
		
		// apply forces
		float speed = moveSpeed * aeiralMult * Time.deltaTime;
		Vector3 xForce = moveRight * move.x * speed;
		Vector3 zForce = moveForward * move.y * speed;
		rb.AddForce(xForce, ForceMode.Acceleration);
		rb.AddForce(zForce, ForceMode.Acceleration);
	}

	Vector3 FindRelativeVelocity() {
		return Quaternion.AngleAxis(transform.eulerAngles.y, groundNormal) * rb.velocity;
	}

	Vector3 FindHorizontalVelocity() {
		return new Vector3(rb.velocity.x, 0, rb.velocity.z);
	}

	// stops/slows movement
	void CounterMove() {
		Vector3 horizVel = FindHorizontalVelocity();
		// makes it move slower if the entity is not moving or the entity is faster than the max speed
		float speed = moveSpeed * aeiralMult * Time.deltaTime;
		if (move.magnitude == 0 || horizVel.magnitude > maxSpeed) {
			// makes counter move smooth
			float frictionX = horizVel.x.Map(-maxSpeed, maxSpeed, -friction, friction);
			float frictionZ = horizVel.z.Map(-maxSpeed, maxSpeed, -friction, friction);
			Vector3 force = new Vector3(frictionX, 0, frictionZ);
			force *= speed;
			// make force negative so it counteracts the movement
			rb.AddForce(-force);

		}
		// slope movement
		if (onGround || wallRiding && !jumping) {
			// restrict upward movement on slopes
			float frictionY = rb.velocity.y.Map(-maxSpeed, maxSpeed, -friction, friction);
			rb.AddForce(-transform.up * frictionY * speed);
			// stick to slope by turning off gravity (may be dangerous)
			rb.useGravity = false;
		}
		else rb.useGravity = true;
	}

	void OnCollisionEnter(Collision info) {
		// add contacts array
		contacts.Add(info.gameObject.GetInstanceID(), info.contacts);
	}

	void OnCollisionStay(Collision info) {
		// update contacts
		contacts[info.gameObject.GetInstanceID()] = info.contacts;
	}

	void OnCollisionExit(Collision info) {
		// remove contacts
		contacts.Remove(info.gameObject.GetInstanceID());
	}
	
	// sets the forward and right directions based on ground normal
	public void UpdateDirsWithGroundNormal() {
		// get a new forward and right from the ground normal (for slopes)
		moveForward = transform.forward;
		Vector3.OrthoNormalize(ref moveForward, ref groundNormal);
		moveRight = Vector3.Cross(groundNormal, moveForward);
	}

	void UpdateGroundNormal() {
		CapsuleCollider cs = GetComponent<CapsuleCollider>();
		// ground check
		float rayDist = cs.height / 2 + checkDist;
		onGround = Physics.Raycast(transform.position, -transform.up, rayDist);

		if (updateNormals) {
			groundNormal = Vector3.zero;

			RaycastHit hit;
			foreach (ContactPoint[] points in contacts.Values) {
				for (int i = 0; i < points.Length; i++) {
					bool didHit = Physics.Raycast(points[i].point + Vector3.up, Vector3.down, out hit, rayDist);
					float slopeAngle = Vector3.Angle(hit.normal, Vector3.up); 

					if (points[i].point.y <= rb.position.y - bottomCSOrigin && didHit && slopeAngle <= stats.maxClimbAngle) {
						groundNormal += hit.normal;
					}
				}
			}

			if (onGround) {
				// normalize the summed up normals
				groundNormal.Normalize();
				UpdateDirsWithGroundNormal();
			}
			else {
				// if not on ground use regular directions
				moveForward = transform.forward;
				moveRight = transform.right;
			}
		}
	}

	void AbleToJump() {
		ableToJump = true;
	}

	void DoJump() {	
		if (jumping && onGround) {
			onGround = false;
			ableToJump = false;
			float jumpForce = stats.jumpForce - rb.velocity.y;
			rb.AddForce(rb.transform.up * jumpForce, ForceMode.Impulse);
		}
	}

	public void StartCrouch() {
		if (ableToCrouch) {
			Debug.Log("Crouch started.");
		}
	}

	public void StopCrouch() {
		Debug.Log("Crouch stopped.");
	}

	public void AbleToCrouch() {
		ableToCrouch = true;
	}
}
