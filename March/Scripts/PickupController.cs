using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour {

	public enum PickupType {
		Wieldable,
		Item,
	}
	
	BoxCollider bc;
	Vector3 lastPos;
	[HideInInspector]
	public bool grounded = false;
	[HideInInspector]
	public bool foundGround = false;
	
	[Header("Ground Settings")]
	public float groundCheck = 1.3f;

	[Header("Pickup Settings")]
	public PickupType type = PickupType.Item;

	void Awake() {
		bc = GetComponentInChildren<BoxCollider>();
		// we are going to make our own gravity
		rotBase = GetRandomRotation();
		newRot = GetRandomRotation();
		grounded = false;
		foundGround = false;
		lastPos = transform.position;

		CollisionCheck();
	}

	void FixedUpdate() {
		CollisionCheck();
		DoRotation();
		DoMovement();
	}

	[Header("Rotation Settings")]
	public bool rotate = true;
	public float rotSpeed = 40f;
	public float smoothSpeed = 2f;
	[HideInInspector] public Vector3 rotBase;
	[HideInInspector] public Vector3 newRot;
	void DoRotation() {
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
	
	[Header("Collision Settings")]
	public LayerMask targetMask;
	void CollisionCheck() {
		RaycastHit hit;
		
		if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheck, targetMask)) {
			grounded = true;
			groundPos = hit.point;
		}
		else grounded = false;

		if (Physics.Linecast(transform.position, lastPos, out hit, targetMask)) {
			groundPos = hit.point;
			grounded = true;
		}
		
		lastPos = transform.position;
	}

	Vector3 GetRandomRotation() {
		return Random.rotation.eulerAngles.normalized;
	}

	[Header("Movement Settings")]
	public float bobOffset = 0.3f;
	public float bobSpeed = 1f;
	public float speed = 4f;
	public Vector3 groundPos;
	void DoMovement() {
		if (grounded)
			foundGround = true;
		else if (!foundGround) {
			transform.position += Physics.gravity * Time.deltaTime;
		}
		
		if (foundGround && grounded) {
			Vector3 pos = groundPos + Vector3.up * (groundCheck - bobOffset);
			// bobs up and down with time
			Vector3 bobPos = Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobOffset;

			transform.position = Vector3.Lerp(transform.position, pos + bobPos, Time.deltaTime * speed);
		}
		else if (foundGround && !grounded) {
			groundPos += Physics.gravity * Time.deltaTime;
		}
	}

	public void Delete() {
		Destroy(gameObject);
	}
}
