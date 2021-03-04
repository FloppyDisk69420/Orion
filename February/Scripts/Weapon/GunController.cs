using UnityEngine;

public class GunController : WeaponController {
	InputManager input;
	public GunProperties props;

	void Start() {
		input = InputController.Instance.input;
	}
	
	override public void OnSwitch(GunProperties newProps) {
		props = newProps;
	}

    override public void Attack() {
		if (gameObject.activeSelf && props) {
			if (props.controller.stats.auto) // if shooting ↓
				props.controller.AutoShoot(input.Player.Fire.ReadValue<float>() > 0);
			// if shoot button clicked
			else if (input.Player.Fire.triggered)
				props.controller.Shoot();
		}
	}
}
