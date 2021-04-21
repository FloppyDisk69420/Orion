using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHolder : MonoBehaviour {
	InputManager input;
	
	[Header("Static")]
	public ComplexRecoil camRecoil;
	public AmmoUIController ammo;
	public GunStats staticStats;
	public Raycaster raycaster;

	[Header("Debug")]
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

		input = InputController.Instance.input;
		// primary is now current weapon if pressed
		input.Player.SwitchPrimary.performed += SwitchToPrimary;
		// secondary is now current weapon if pressed
		input.Player.SwitchSecondary.performed += SwitchToSecondary;
		// melee is now current weapon if pressed
		input.Player.SwitchMelee.performed += SwitchToMelee;
		// can switch to primary, secondary, and melee weapons
		input.Player.SwitchWeapon.performed += SwitchWeapon;
		UpdateWeapons();
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
	void AssignADS(GunController gun) {
		ADSController aim = GetComponent<ADSController>();
		if (gun) aim.gunProps = gun.props;
		else aim.gunProps = null;
		// makes the controller update states.
		aim.OnDefault();
	}

	void UpdateUI(GunController gun) {
		if (gun) ammo.props = gun.props;
		else ammo.props = null;
	}

	void UpdateBullet(GunController gun) {	
		if (gun) raycaster.bulletPrefab = gun.props.bullet;
		else raycaster.bulletPrefab = null;
	}

	public GunController GetGun() {
		switch (weaponType) {
			case WeaponType.Primary:
				return primaryWeapon;

			case WeaponType.Secondary:
				return secondaryWeapon;
		}
		return null;
	}

	void UpdateRecoil(GunController gun) {
		if (gun) {
			currentWeapon = gun;
			if (gun.props)
				camRecoil.stats = gun.props.stats;
			else camRecoil.stats = staticStats;
		}
		else {
			currentWeapon = null;
			camRecoil.stats = staticStats;
		}
	}

	public void UpdateWeapons() {
		GunController curGun = GetGun();
		ActivateCurrent();
		
		AssignADS(curGun);
		UpdateUI(curGun);
		UpdateBullet(curGun);
		UpdateRecoil(curGun);
	}
}
