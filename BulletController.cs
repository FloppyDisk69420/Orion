using UnityEngine;

public class BulletController : MonoBehaviour {
	public bool isReady = false;
	public bool start = false;
	public GunStats stats;
	public ParticleSystem trail;
	Vector3 firstPos;
	Rigidbody rb;
	
	void Start() {
		rb = GetComponent<Rigidbody>();
		Collider[] colliders = FindObjectsOfType<Collider>();
		foreach (Collider collider in colliders) {
			if (!collider.CompareTag("Collider")) {
				Physics.IgnoreCollision(GetComponent<Collider>(), collider);
			}
		}
		rb.useGravity = false;
		rb.isKinematic = false;
		rb.drag = 1;
		rb.detectCollisions = true;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		firstPos = transform.position;
	}

    void FixedUpdate() {
		if (isReady) {
			float dx = Mathf.Pow(transform.position.x - firstPos.x, 2);
			float dy = Mathf.Pow(transform.position.y - firstPos.y, 2);
			float dz = Mathf.Pow(transform.position.z - firstPos.z, 2);
			if (dx + dy + dz > Mathf.Pow(stats.range, 2)) {
				Debug.Log("You shot nothing!");
				Destroy(gameObject);
			}
			rb.velocity = transform.forward * stats.bulletSpeed * Time.fixedDeltaTime;
		}
    }

	void OnCollisionEnter(Collision collision) {
		if (collision.transform.CompareTag("Collider")) {
			Debug.Log("You shot " + collision.collider.name + "!");
			Destroy(gameObject);
		}
	}
}
