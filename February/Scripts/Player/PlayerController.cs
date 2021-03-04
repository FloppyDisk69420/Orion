using UnityEngine;

public class PlayerController : MonoBehaviour {
	InputManager input;
	[Header("Assignables")]
	[SerializeField] Movement movement;
	[SerializeField] PlayerLook Plook;
	[SerializeField] WallRide wallRide;
	[Header("Debug")]
	public bool cursorVis = false;
	
	void Awake() {
		movement = GetComponent<Movement>();
		wallRide = GetComponent<WallRide>();
	}

	void Start() {
		input = InputController.Instance.input;
		Cursor.visible = false;
		cursorVis = Cursor.visible;
		Cursor.lockState = CursorLockMode.Locked;

		input.Player.Jump.performed += _ => wallRide.WallJump();
		input.Player.Crouch.performed += _ => StartCrouch();
		input.Player.Crouch.canceled += _ => StopCrouch();
	}

	void Update() {
		CheckInput();
	}

	void CheckInput() {
		movement.move = input.Player.Movement.ReadValue<Vector2>();
		wallRide.move = input.Player.Movement.ReadValue<Vector2>();
		Plook.look = input.Player.Look.ReadValue<Vector2>() * Plook.sensitivity * Time.fixedDeltaTime;
		movement.wallRiding = wallRide.wallRiding;
		// if button is held down
		movement.sprinting = input.Player.Sprint.ReadValue<float>() > 0;
		movement.jumping = input.Player.Jump.ReadValue<float>() > 0;
		movement.crouching = input.Player.Crouch.ReadValue<float>() > 0;
	}

	void StartCrouch() {
		if (!wallRide.wallRiding)
			movement.StartCrouch();
	}

	void StopCrouch() {
		if (!wallRide.wallRiding)
			movement.StopCrouch();
	}
}
