using System.Collections;
using UnityEngine;
// TODO does full cycle and player not there sus goes to DoRoute level
public class AIMovement : MonoBehaviour {
	[Header("Serialize")]
	public LayerMask otherEnemies;
	public GameObject weapon;
	public Route route;
	public Transform target;
	public EnemyStats stats;
	public LayerMask whatIsGround;

	Rigidbody rb;
	//Movement
	bool cancellingGrounded;
	bool grounded;
	float maxSpeed;
	float moveSpeed;
	float counterMovement = 0.175f;
	float threshold = 0.01f;
	//Crouch & Slide
	Vector3 crouchScale = new Vector3(1, 0.5f, 1);
	Vector3 playerScale;
	float slideCounterMovement = 0.2f;
	//Jumping
	bool readyToJump = true;
	//Input
	[Header("Movement Vars")]
	public Vector2 move;
	public bool jumping, sprinting, crouching;
	//Sliding
	Vector3 normalVector = Vector3.up;

	void StartCrouch() {
		transform.localScale = crouchScale;
		transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
		if (rb.velocity.magnitude > 0.5f) {
			if (grounded) {
				rb.AddForce(transform.forward * stats.slideSpeed);
			}
		}
	}

	void StopCrouch() {
		transform.localScale = playerScale;
		transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
	}

	void DoMovement() {
		//Extra gravity
		rb.AddForce(Vector3.down * Time.deltaTime * 10);
		
		//Find actual velocity relative to where player is looking
		Vector2 mag = FindVelRelativeToLook();
		float xMag = mag.x, yMag = mag.y;

		//Counteract sliding and sloppy movement
		CounterMovement(move.x, move.y, mag);
		
		//If holding jump && ready to jump, then jump
		if (readyToJump && jumping) Jump();

		//Set speeds
		if (sprinting) {
			maxSpeed = stats.maxSprintSpeed * stats.agility;
			moveSpeed = stats.sprintSpeed;
		}
		else {
			maxSpeed = stats.maxWalkSpeed * stats.agility;
			moveSpeed = stats.walkSpeed;
		}
		
		//If sliding down a ramp, add force down so player stays grounded and also builds speed
		if (crouching && grounded && readyToJump) {
			rb.AddForce(Vector3.down * Time.deltaTime * 3000);
			return;
		}
		
		//If speed is larger than maxspeed, cancel out the input so you don't go over max speed
		if (move.x > 0 && xMag > maxSpeed) move.x = 0;
		if (move.x < 0 && xMag < -maxSpeed) move.x = 0;
		if (move.y > 0 && yMag > maxSpeed) move.y = 0;
		if (move.y < 0 && yMag < -maxSpeed) move.y = 0;

		//Some multipliers
		float multiplier = 1f, multiplierV = 1f;
		
		// Movement in air
		if (!grounded) {
			multiplier = 0.5f;
			multiplierV = 0.5f;
		}
		
		// Movement while sliding
		if (grounded && crouching) multiplierV = 0f;

		//Apply forces to move player
		Vector3 speedY = transform.forward * move.y * moveSpeed * Time.deltaTime * multiplier * multiplierV;
		Vector3 speedX = transform.right * move.x * moveSpeed * Time.deltaTime * multiplier;
		rb.AddForce(speedY);
		rb.AddForce(speedX);
	}

	void Jump() {
		if (grounded && readyToJump) {
			readyToJump = false;

			//Add jump forces
			rb.AddForce(Vector2.up * stats.jumpForce * 1.5f);
			rb.AddForce(normalVector * stats.jumpForce * 0.5f);
			
			//If jumping while falling, reset y velocity.
			Vector3 vel = rb.velocity;
			if (rb.velocity.y < 0.5f)
				rb.velocity = new Vector3(vel.x, 0, vel.z);
			else if (rb.velocity.y > 0) 
				rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
			
			Invoke(nameof(ResetJump), stats.jumpCooldown);
		}
	}
		
	void ResetJump() {
		readyToJump = true;
	}

	void CounterMovement(float x, float y, Vector2 mag) {
		if (!grounded || jumping) return;

		//Slow down sliding
		if (crouching) {
			rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
			return;
		}

		//Counter movement
		if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
			rb.AddForce(moveSpeed * transform.right * Time.deltaTime * -mag.x * counterMovement);
		}
		if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
			rb.AddForce(moveSpeed * transform.forward * Time.deltaTime * -mag.y * counterMovement);
		}
		
		//Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
		if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
			float fallspeed = rb.velocity.y;
			Vector3 n = rb.velocity.normalized * maxSpeed;
			rb.velocity = new Vector3(n.x, fallspeed, n.z);
		}
	}

	Vector2 FindVelRelativeToLook() {
		float lookAngle = transform.eulerAngles.y;
		float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

		float u = Mathf.DeltaAngle(lookAngle, moveAngle);
		float v = 90 - u;

		float magnitue = rb.velocity.magnitude;
		float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
		float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);
		
		return new Vector2(xMag, yMag);
	}

	bool IsFloor(Vector3 v) {
		float angle = Vector3.Angle(Vector3.up, v);
		return angle < stats.maxClimbAngle;
	}

	void StopGrounded() {
		grounded = false;
	}

	void Awake() {
		rb = GetComponent<Rigidbody>();
	}
		
	void Start() {
		playerScale =  transform.localScale;
		i = 0;
		onRoute = true;
	}

	public void OnCollisionStay(Collision other) {
		//Make sure we are only checking for walkable layers
		int layer = other.gameObject.layer;
		if (whatIsGround != (whatIsGround | (1 << layer))) return;

		//Iterate through every collision in a physics update
		for (int i = 0; i < other.contactCount; i++) {
			Vector3 normal = other.contacts[i].normal;
			//FLOOR
			if (this.IsFloor(normal)) {
				grounded = true;
				cancellingGrounded = false;
				normalVector = normal;
				CancelInvoke(nameof(StopGrounded));
			}
		}

		//Invoke ground/wall cancel, since we can't check normals with CollisionExit
		float delay = 3f;
		if (!cancellingGrounded) {
			cancellingGrounded = true;
			Invoke(nameof(StopGrounded), Time.deltaTime * delay);
		}
	}

	

	[Header("AI Vars")]
	public float lookSpeed = 2f;
	public float callRadius = 50f;
	public bool onRoute = true;
	
	int i;
	Vector3 susPoint;
	bool callBackup = false;

	void CheckStates() {
		DiscoverPlayer dp = GetComponent<DiscoverPlayer>();
		switch (stats.state) {
			case EnemyState.DoRoute:
				onRoute = true;
			break;
			case EnemyState.KindaSussed:
				if (dp.targetInView) susPoint = target.position;
				onRoute = false;
			break;
			case EnemyState.Sussed:
				if (dp.targetInView) susPoint = target.position;
				onRoute = false;
			break;
			case EnemyState.Alerted:
				if (dp.targetInView) susPoint = target.position;
				onRoute = false;
			break;
		}
	}

	void FixedUpdate() {
		if (stats.state == EnemyState.DoRoute) {
			if (route.checkpoints.Count > 0) {
				DoRoute();
			}
		}
		DoMovement();
		TurnTo(WhereToTurn());
	}

	void Update() {
		BackupCheck();
		DoStates();
	}

	[HideInInspector()]
	public bool alreadyCalled = false;
	void BackupCheck() {
		if (stats.state == EnemyState.Alerted && callBackup) {
			Collider[] enemies = Physics.OverlapSphere(transform.position, callRadius, otherEnemies);
			foreach (Collider enemy in enemies) {
				enemy.GetComponent<DiscoverPlayer>().susCounter = 1;
				// makes it so that there is not a chain reaction.
				enemy.GetComponent<AIMovement>().alreadyCalled = true;
			}
		}
	}

	void DoRoute() {
		if (HitCheckpoint()) {
			if (i < route.checkpoints.Count - 1) {
				i++;
			}
			else {
				i = 0;
			}
			StartCoroutine(Wait(route.times[i]));
		}
		if (stats.state != EnemyState.Idle) {
			move.y = 1;
		}
		else {
			move.y = 0;
		}
		TurnTo(route.checkpoints[i]);
	}

	void DoStates() {
		DiscoverPlayer dp = GetComponent<DiscoverPlayer>();
		dp.DoStates();
		CheckStates();
		UpdateMovement();
	}

	IEnumerator Wait(float time) {
		EnemyState currState = stats.state;
		stats.state = EnemyState.Idle;
		yield return new WaitForSeconds(time);
		stats.state = currState;
	}

	bool HitCheckpoint() {
		Vector2 topDownEnemy = new Vector2(transform.position.x, transform.position.z);
		Vector2 topDownRoute = new Vector2(route.checkpoints[i].x, route.checkpoints[i].z);
		float dist = Vector2.Distance(topDownEnemy, topDownRoute);
		return (dist < 0.5f);
	}

	void TurnTo(Vector3 position) {
		Quaternion targetLook = Quaternion.LookRotation(position - transform.position);
		// Only get y rotation
		Quaternion look = Quaternion.Euler(0, targetLook.eulerAngles.y, 0);
		transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * lookSpeed);
	}

	Vector3 WhereToTurn() {
		switch (stats.state) {
			case EnemyState.DoRoute: return route.checkpoints[i];
			case EnemyState.KindaSussed: return susPoint;
			case EnemyState.Sussed: return susPoint;
			case EnemyState.Alerted: return susPoint;
			case EnemyState.Idle: 
				if (onRoute) return route.checkpoints[i];
				else return LookAround();
		}
		// dont turn
		return transform.forward;
	}

	void UpdateMovement() {
		switch (stats.state) {
			case EnemyState.DoRoute:
				if (route.checkpoints.Count > 0) DoRoute();
			break;
			case EnemyState.KindaSussed:
				stats.maxWalkSpeed = Random.Range(3, 7);
				EnemyMovement();
			break;
			case EnemyState.Sussed:
				// more careful
				stats.maxWalkSpeed = Random.Range(3, 6);
				EnemyMovement();
			break;
			case EnemyState.Alerted:
				// more careful
				stats.maxWalkSpeed = Random.Range(4, 8);;
				callBackup = true;
				EnemyMovement();
			break;
		}
	}

	public float distToKeep = 10f;
	public float safeZone = 1f;
	void EnemyMovement() {
		float dist = Vector3.Distance(transform.position, susPoint);
		if (stats.state == EnemyState.KindaSussed || stats.state == EnemyState.Sussed) {

		}
		if (stats.state == EnemyState.Alerted) {
			// keeps distance
			if (dist < distToKeep) {
				move.y = -1;
			}
			else if (dist > distToKeep && dist < distToKeep + safeZone) {
				// circles around player
				move.x = 1;
			}
			else {
				move.y = 1;
			}
		}
	}

	// look around helper variables
	bool lookedAtRandomPoint = true;
	bool toRight = true;
	Vector3 pointToLook;
	Vector3 LookAround() {
		if (pointToLook.y == transform.rotation.eulerAngles.y) {
			lookedAtRandomPoint = true;
		}
		
		if (lookedAtRandomPoint) {
			Vector3 currRot = transform.rotation.eulerAngles;
			Vector3 addedRot = Vector3.zero;
			if (toRight) addedRot.y += Random.Range(15, 90);
			else addedRot.y -= Random.Range(15, 90);
			pointToLook = currRot + addedRot;
			lookedAtRandomPoint = false;
		}
		return pointToLook;
	}
}
