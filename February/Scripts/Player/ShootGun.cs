using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootGun : MonoBehaviour {
	public GunStats stats;
	public RecoilController recoil;
	public ComplexRecoil camRecoil;
	public Raycaster raycaster;
	public ParticleSystem muzzleFlash;
	float lastFiredTime = 0f;

	void Start() {
		recoil = GetComponent<RecoilController>();
		// get stats of gun
		stats = GetComponent<GunStats>();
	}

	void FixedUpdate() {
		camRecoil.UpdateRecoil();
		recoil.UpdateRecoil();
	}

	public void Shoot() {
		if (Time.time >= lastFiredTime) {
			camRecoil.fire = true;
			recoil.fire = true;
			muzzleFlash.Play();
			raycaster.ShootBullet(stats, muzzleFlash.transform);
			lastFiredTime = Time.time + stats.fireRate;
		}
	}

	public void AutoShoot(bool isDown) {
		// while the fire key is down...
		if (isDown) Shoot();
	}
}
