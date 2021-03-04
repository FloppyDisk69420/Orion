using UnityEngine;

public class BulletController : MonoBehaviour {
	[HideInInspector]
	public GunStats stats;
	[HideInInspector]
	public LayerMask targetMask;
	public BulletImpact bt;
	
	Rigidbody rb;
	Vector3 firstPos;
	Vector3 lastPos;
	float radius;

	public float forceMult = 0.5f;
	public float dist = 5f;
	public float expRadius = 10f;
	public float distMult = 1f;

	void Awake() {
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		rb.freezeRotation = true;
		firstPos = transform.position;
		lastPos = transform.position;

		BoxCollider bc = GetComponent<BoxCollider>();
		// not exact
		radius = (bc.size.x + bc.size.y) / 2;
	}

	void FixedUpdate() {
		DoBulletVelocity();
		CheckCollision();
	}

	void DoBulletVelocity() {
		float factor = 1 - rb.velocity.magnitude.Map(0, stats.bulletSpeed, 0, 1);
		
		rb.AddForce(rb.transform.forward * stats.bulletSpeed * factor);
		rb.velocity = Vector3.ClampMagnitude(rb.velocity, stats.bulletSpeed);
	}

	// checks collision with a spherecast from the last position to current position.
	void CheckCollision() {
		Vector3 currPos = rb.position;
		
		RaycastHit hit;
		float dist = Vector3.Distance(lastPos, currPos);
		if (Physics.SphereCast(lastPos, radius, transform.forward, out hit, dist, targetMask)) {
			bt.OnImpact(hit);
			Destroy(gameObject);
		}
		// update pos
		lastPos = currPos;
	}

	void Update() {
		if (Vector3.Distance(rb.position, firstPos) >= stats.range) {
			Debug.Log("Shot nothing.");
			Destroy(gameObject);
		}
	}

}
