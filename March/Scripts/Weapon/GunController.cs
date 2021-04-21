using UnityEngine;

public class GunController : WeaponController {
	InputManager input;
	public GunProperties props;
	public bool reserved = false;

	void Awake() {
		UpdateWeapon();
	}

	void Start() {
		input = InputController.Instance.input;
	}

	public void UpdateWeapon() {
		if (transform.childCount > 0) {
			props = transform.GetChild(0).GetComponent<GunProperties>();
			reserved = true;
		}
		else {
			props = null;
			reserved = false;
		}
	}

    override public void Attack() {
		if (gameObject.activeSelf && props && input != null) {
			if (props.controller.stats.auto) // if shooting ↓
				props.controller.AutoShoot(input.Player.Fire.ReadValue<float>() > 0);
			// if shoot button clicked
			else if (input.Player.Fire.triggered)
				props.controller.Shoot();

			if (input.Player.Reload.triggered) {
				props.controller.doReload = true;
			}
		}
	}
}
