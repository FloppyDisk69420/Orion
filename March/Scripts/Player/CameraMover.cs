using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
	[Header("Assign")]
	public Transform cam;
	public Rigidbody rb;
	public Transform returnPos;
	[Header("Settings")]
	[Tooltip("Constant multiplier of the camera movement speed.")]
	public float returnSpeed = 2f;
	[Tooltip("If this is low, the camera movement will be loose. Otherwise, it will be snappy.")]
	public float snapness = 2f;
	[Tooltip("The maximum distance allowed between the camera position and the player position.")]
	public float maxAllowedDist = 2f;
	[Tooltip("The minimum speed the camera goes")]
	public float minSpeed = 0.1f;
	[Tooltip("The minimum velocity change that must occur to make the camera bounce.")]
	public float countedDist = 4f;

	[Header("Readonly")]
	public Vector3 speed;
	public Vector3 smoothSpeed;
	public Vector3 lastVel;

	void Start() {
		lastVel = Vector3.zero;
	}

	void Update() {
		DoCamMovement();
	}

	void DoCamMovement() {
		Transform t = cam.transform;
		float velDiff = Vector3.Distance(lastVel, rb.velocity);
		// add speed to the camera if sudden stop occurs
		if (velDiff >= countedDist) {
			// avoid going over maxAllowedDist
			Vector3 addedSpeed = lastVel - rb.velocity;
			addedSpeed = Vector3.ClampMagnitude(addedSpeed, maxAllowedDist);
			// avoid clipping through obstacles
			RaycastHit hit;
			Physics.Raycast(t.position, addedSpeed.normalized, out hit, maxAllowedDist);
			addedSpeed = Vector3.ClampMagnitude(addedSpeed, hit.distance);
			// apply speed
			speed += addedSpeed;
		}
		// set speed smoothly to zero
		float snapMult = velDiff.Map(0, maxAllowedDist, minSpeed, snapness);
		speed = Vector3.Lerp(speed, Vector3.zero, Time.unscaledDeltaTime * returnSpeed * snapMult);
		// smoothly transition cam movement
		smoothSpeed = Vector3.Lerp(smoothSpeed, speed, Time.unscaledDeltaTime * returnSpeed);

		lastVel = rb.velocity;
		t.position = returnPos.position + smoothSpeed;
	}
}
