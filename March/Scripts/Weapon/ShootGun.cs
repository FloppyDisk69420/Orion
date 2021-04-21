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
	public int rounds;
	public bool reloading = false;
	public bool doReload = false;

	void Start() {
		recoil = GetComponent<RecoilController>();
		// get stats of gun
		stats = GetComponent<GunStats>();

		reloading = false;
		doReload = false;
		rounds = stats.magazine;
		curReloadTime = Time.unscaledTime;
	}

	float curReloadTime = 0f;
	void FixedUpdate() {
		camRecoil.UpdateRecoil();
		recoil.UpdateRecoil();
		ReloadCheck();
	}

	public void ReloadCheck() {
		// start reload if there are no more rounds
		if (rounds <= 0) doReload = true;
		
		// check if player still has more ammo
		if (stats.ammo > 0) {
			// done once
			if (doReload && !reloading && rounds < stats.magazine) {
				reloading = true;
				curReloadTime = Time.unscaledTime;
			}
			// done while reloading
			else if (doReload) {
				// check if done
				float curTime = Time.unscaledTime;
				float targetTime = curReloadTime + stats.reloadTime;
				if (curTime >= targetTime) {
					// if ammo is 1 and full reload is 3, 1 is chosen because it is the least
					int roundsAdded = Mathf.Min(stats.ammo, stats.magazine - rounds);
					rounds += roundsAdded;
					stats.ammo -= roundsAdded;
					doReload = false;
				}
				else {
					float offset = curTime - curReloadTime;
					offset = offset.Map(0, stats.reloadTime, 0, 1);
					
					transform.localRotation = Quaternion.Euler(360 * offset, 0, 0);
				}
			}
			// done once not reloading
			else if (!doReload && reloading) {
				reloading = false;
				transform.localRotation = Quaternion.Euler(0, 0, 0);
			}
			// if no criteria is met then cancel reload
			else {
				doReload = false;
			}
		}
	}

	public void Shoot() {
		if (Time.unscaledTime >= lastFiredTime && !reloading && rounds > 0) {
			// update recoils
			camRecoil.fire = true;
			recoil.fire = true;
			// plays the shoot effect
			muzzleFlash.Play();
			// shoots the actual bullet
			raycaster.ShootBullet(stats, muzzleFlash.transform);
			rounds -= Mathf.Min(stats.shotCost, rounds);

			// update time
			lastFiredTime = Time.unscaledTime + stats.fireRate;
		}
	}

	public void AutoShoot(bool isDown) {
		// while the fire key is down...
		if (isDown) Shoot();
	}
}
