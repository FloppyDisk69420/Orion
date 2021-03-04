using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHolder : MonoBehaviour {
	InputManager input;
	
	[SerializeField] ComplexRecoil camRecoil;
	public GunController primaryWeapon;
	public GunController secondaryWeapon;
	public MeleeController meleeWeapon;

	[HideInInspector] public WeaponController currentWeapon;

	int weaponTypeLength = 3;
	public enum WeaponType {
		Primary,
		Secondary,
		Melee,
	}
	public WeaponType weaponType;

	void Start() {
		weaponType = WeaponType.Primary;
		UpdateWeapons();

		input = InputController.Instance.input;
		// primary is now current weapon if pressed
		input.Player.SwitchPrimary.performed += SwitchToPrimary;
		// secondary is now current weapon if pressed
		input.Player.SwitchSecondary.performed += SwitchToSecondary;
		// can switch to primary, secondary, and melee weapons
		input.Player.SwitchWeapon.performed += SwitchWeapon;
	}

	void Update() {
		AttackCheck();
	}

	void AttackCheck() {
		if (currentWeapon)
			currentWeapon.Attack();
	}

	void SwitchToPrimary(InputAction.CallbackContext ctx) {
		weaponType = WeaponType.Primary;
		UpdateWeapons();
	}

	void SwitchToSecondary(InputAction.CallbackContext ctx) {
		weaponType = WeaponType.Secondary;
		UpdateWeapons();
	}

	void SwitchToMelee(InputAction.CallbackContext ctx) {
		weaponType = WeaponType.Melee;
		UpdateWeapons();
	}

	void SwitchWeapon(InputAction.CallbackContext ctx) {
		// scroll constant when scrolling (idk why its a thing)
		float scrollConstant = 120;
		int index = (int)weaponType;

		index -= Mathf.FloorToInt(ctx.ReadValue<float>() / scrollConstant);
		index %= weaponTypeLength;
		if (index < 0)
			index += weaponTypeLength;

		weaponType = (WeaponType)index;
		
		UpdateWeapons();
	}

	void ActivateCurrent() {
		// checks which weapon is current
		if (primaryWeapon)
			primaryWeapon.gameObject.SetActive(weaponType == WeaponType.Primary);
		if (secondaryWeapon)
			secondaryWeapon.gameObject.SetActive(weaponType == WeaponType.Secondary);
		if (meleeWeapon)
			meleeWeapon.gameObject.SetActive(weaponType == WeaponType.Melee);
	}

	// wish it was more simple to assign but this is the easiest way
	void AssignADS() {
		ADSController aim = GetComponent<ADSController>();
		switch (weaponType) {
			case WeaponType.Primary:
				if (primaryWeapon)
					aim.gunProps = primaryWeapon.props;
			break;
			case WeaponType.Secondary:
				if (secondaryWeapon)
					aim.gunProps = secondaryWeapon.props;
			break;
		}
		// makes the controller update states.
		aim.OnDefault();
	}

	public void UpdateWeapons() {
		ActivateCurrent();
		AssignADS();

		switch (weaponType) {
			case WeaponType.Primary:
				if (primaryWeapon.props) {
					currentWeapon = primaryWeapon;
					camRecoil.stats = primaryWeapon.props.stats;
				}
			break;
			case WeaponType.Secondary:
				if (secondaryWeapon.props) {
					currentWeapon = secondaryWeapon;
					camRecoil.stats = secondaryWeapon.props.stats;
				}
			break;
			case WeaponType.Melee:
				if (meleeWeapon) {
					currentWeapon = meleeWeapon;
				}
			break;
		}
	}
}
