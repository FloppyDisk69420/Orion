using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootGun : MonoBehaviour {
	InputManager input;
	GunStats stats;
    bool equipped;

	public RecoilController recoil;
	public ComplexRecoil camRecoil;
	public Raycaster raycaster;
	public ParticleSystem muzzleFlash;
	float lastFiredTime = 0f;

	void Start() {
		// get controls manager
		input = InputController.Instance.input;

		recoil = GetComponent<RecoilController>();
		// get stats of gun
		stats = GetComponent<GunStats>();

		if (stats.auto) input.Player.Fire.performed += ctx => StartShooting(ctx);
		else input.Player.Fire.performed += ctx => Shoot();
	}

	void FixedUpdate() {
		camRecoil.UpdateRecoil();
		recoil.UpdateRecoil();
	}

	void Shoot() {
		if (Time.time >= lastFiredTime) {
			camRecoil.fire = true;
			recoil.fire = true;
			muzzleFlash.Play();
			raycaster.ShootBullet(stats, muzzleFlash.transform);
			lastFiredTime = Time.time + stats.fireRate;
		}
	}

	IEnumerator AutoShoot(InputAction.CallbackContext callback) {
		// while the fire key is down...
		while (input.Player.Fire.ReadValue<float>() > InputSystem.settings.defaultButtonPressPoint) {
			// shoot
			Shoot();
			yield return null;
		}
	}

	// for automatic guns. i.e. assault rifles.
	Coroutine automaticShootProcess;
	public void StartShooting(InputAction.CallbackContext callback) {
		if (automaticShootProcess != null) {
			StopCoroutine(automaticShootProcess);
		}
		automaticShootProcess = StartCoroutine(AutoShoot(callback));
	}
}
