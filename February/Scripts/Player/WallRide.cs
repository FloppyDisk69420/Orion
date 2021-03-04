using UnityEngine;

[RequireComponent(typeof(Movement))]
public class WallRide : MonoBehaviour {
	[Header("Assignables")]
	[SerializeField] Rigidbody rb;
	[SerializeField] Transform cam;
	[SerializeField] Movement movement;
	[SerializeField] EntityStats stats;
	[SerializeField] Transform tilter;

	[Header("Debug")]
	public bool wallLeft;
	public bool wallRight;
	public bool wallRiding;
	public float checkDist = 1.25f;
	public float friction = 10f;
	
	[Header("Movement Settings")]
	public Vector2 move;
	public float wallForce = 15f;
	public float jumpForceMult = 2f;
	public float maxWallSpeed;
	public float wallSpeed;
	public enum WallJumpState {
		NotStarted,
		Started,
		Finished,
	}

	[Header("Camera Settings")]
	public float maxCamTilt = 30f;
	public float tiltSpeed = 2f;

	void Awake() {
		rb = GetComponent<Rigidbody>();
		movement = GetComponent<Movement>();
		stats = GetComponent<EntityStats>();
	}

	void Start() {
		wallLeft = false;
		wallRight = false;
		move = new Vector2();
	}

	void Update() {
		WallCheck();
	}

	void FixedUpdate() {
		TiltCamera();
		if (wallRiding) DoWallRide();
		CounteractUpwards();
	}

	public void WallJump() {
		if (wallRiding) {
			// cancel current upwards motion
			rb.AddForce(Vector3.down * rb.velocity.y, ForceMode.VelocityChange); 
			// do jump
			rb.AddForce(Vector3.up * stats.jumpForce * jumpForceMult, ForceMode.Impulse);			

			wallJumpState = WallJumpState.Started;
			StopWallRide();
		}
	}
	
	// used for better jump movement
	WallJumpState wallJumpState = WallJumpState.NotStarted;
	void WallCheck() {	
		//check if wall is (still) there
		wallLeft = Physics.Raycast(transform.position, -transform.right, checkDist) && !movement.onGround;
		wallRight = Physics.Raycast(transform.position, transform.right, checkDist) && !movement.onGround;

		if (!wallLeft && !wallRight) StopWallRide();
		else StartWallRide();

		switch (wallJumpState) {
			case WallJumpState.Started:
				if (!wallLeft && !wallRight)
					wallJumpState = WallJumpState.Finished;
			break;
			case WallJumpState.Finished:
				wallJumpState = WallJumpState.NotStarted;
			break;
		}
	}

	void StartWallRide() {
		rb.useGravity = false;
		wallRiding = true;
		// makes it so that it stops movement on the walls
		movement.alwaysDoFriction = true;
		// we are going to set the normals and we don't want them to be reset
		movement.updateNormals = false;
	}

	void UpdateNormals() {
		if (wallJumpState == WallJumpState.Started) {
			movement.updateNormals = true;
		}
		else {
			movement.groundNormal = Vector3.zero;
			RaycastHit hit;
			// get wall normal
			if (wallLeft) Physics.Raycast(transform.position, -transform.right, out hit, checkDist);
			else Physics.Raycast(transform.position, transform.right, out hit, checkDist);

			movement.groundNormal += hit.normal;
			movement.groundNormal.Normalize();
			// get forward and right
			movement.UpdateDirsWithGroundNormal();
		}
	}

	void DoWallRide() {
		// makes Movement Component do movement on wall
		UpdateNormals();
	}

	void CounteractUpwards() {
		// if (wallRiding) {
		// 	float speed = stats.walkSpeed;
		// 	if (movement.sprinting) speed = stats.sprintSpeed;
		// 	// counteracts upward motion if any
		// 	float threshold = rb.velocity.y.Map(-maxWallSpeed, maxWallSpeed, -friction, friction);
		// 	rb.AddForce(speed * -rb.transform.up * Time.deltaTime * threshold);
		// }
	}

	void StopWallRide() {
		rb.useGravity = true;
		wallRiding = false;
		movement.alwaysDoFriction = false;
		movement.updateNormals = true;
	}

	void TiltCamera() {
		Quaternion toRot;

		if (wallLeft) toRot = Quaternion.Euler(0, 0, -maxCamTilt);
		else if (wallRight) toRot = Quaternion.Euler(0, 0, maxCamTilt);
		else toRot = Quaternion.Euler(0, 0, 0);

		tilter.localRotation = Quaternion.Slerp(tilter.localRotation, toRot, Time.deltaTime * tiltSpeed);
	}
}
