using System.Collections;
using UnityEngine;

public class ThrowableObject : MonoBehaviour {

	public enum PickupType {
		Grab,
		Hold,
	}

	private InputManager input;
	
	public Transform cam;
	[HideInInspector]
	public Rigidbody rb;
	[HideInInspector]
	public Collider col;
	public LayerMask targetMask;

	[Header("Pickup Settings")]
	[Tooltip("If true, then the player can interact with this and pick this object up.")]
	public bool pickupable = false;
	[Tooltip("If set to Grab, then the object moves towards the player's cursor slowly.\nIf set to Hold, then the object moves to the player's cursor immediately.")]
	public PickupType pickupType = PickupType.Hold;
	public float pickupDistance = 3f;
	public float pickupPower = 2f;
	public float pickupMag = 15f;
	public float maxPickupDist = 5f;
	public bool isPickedUp = false;

	[Header("Collision Settings")] 
	public float skinWidth = 0.1f; 
	private float minimumExtent; 
	private float partialExtent; 
	private float sqrMinimumExtent; 
	private Vector3 previousPosition;

	void Awake() {
		whenCollided = DefaultWhenCollided;
	}

	void Start() {
		input = InputController.Instance.input;

		rb = GetComponent<Rigidbody>();
		isPickedUp = false;

		// Collision detection things
		col = GetComponentInChildren<Collider>();
		previousPosition = rb.position; 
		minimumExtent = Mathf.Min(Mathf.Min(col.bounds.extents.x, col.bounds.extents.y), col.bounds.extents.z); 
		partialExtent = minimumExtent * (1.0f - skinWidth); 
		sqrMinimumExtent = minimumExtent * minimumExtent; 
	}
	
	void FixedUpdate() {
		CollisionCheck();
	}

	public System.Action<RaycastHit> whenCollided;

	public void DefaultWhenCollided(RaycastHit hit) {
		Debug.Log("Collided with " + name + ".");
	}

	// basically DontGoThroughThings component
	void CollisionCheck() {
		//have we moved more than our minimum extent? 
		Vector3 movementThisStep = rb.position - previousPosition; 
		float movementSqrMagnitude = movementThisStep.sqrMagnitude;
 
		if (movementSqrMagnitude > sqrMinimumExtent) {
			float movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
			RaycastHit hitInfo; 
 
			//check for obstructions we might have missed 
			if (Physics.Raycast(previousPosition, movementThisStep, out hitInfo, movementMagnitude, targetMask.value)) {
				if (!hitInfo.collider)
					return;

				if (hitInfo.collider.isTrigger) 
					hitInfo.collider.SendMessage("OnTriggerEnter", col);

				if (!hitInfo.collider.isTrigger)
					rb.position = hitInfo.point - (movementThisStep / movementMagnitude) * partialExtent;
				
				whenCollided(hitInfo);
			}
		}
 
		previousPosition = rb.position;
	}
}