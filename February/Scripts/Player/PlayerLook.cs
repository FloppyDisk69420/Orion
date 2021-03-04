using UnityEngine;

public class PlayerLook : MonoBehaviour {
	[Header("Assignables")]
    [SerializeField] Transform cam;
	[Header("Input")]
	public Vector2 look = new Vector2();
	public float sensitivity = 100f;
	public float maxLookUp = 90f;

	float xRot = 0f;

	void Start() {
		look = Vector2.zero;
		xRot = 0f;
	}
	
	void Update() {
		Look();
	}

	float desiredX = 0f;
	void Look() {        
        // make rotation not over/under flow from max view angles
        xRot -= look.y;
        xRot = Mathf.Clamp(xRot, -maxLookUp, maxLookUp);

		desiredX += look.x;
        //Perform the rotations
        cam.transform.localRotation = Quaternion.Euler(xRot, desiredX, 0);
        transform.localRotation = Quaternion.Euler(0, desiredX, 0);
	}
}
