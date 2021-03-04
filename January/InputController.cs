using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour {
	private static InputController _instance;
    public InputManager input;

	public static InputController Instance {
		get {
			return _instance;
		}
	}

	void Awake() {
		if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
		}
		else {
			_instance = this;
		}
		input = new InputManager();
	}

	void OnEnable() {
		input.Enable();
	}

	void OnDisable() {
		input.Disable();
	}
}
