using UnityEngine;

public class AIShoot : MonoBehaviour {
	[Header("Serialize")]
	[SerializeField] GunStats stats;
	[SerializeField] RecoilController recoil;
	[SerializeField] Raycaster raycaster;
	[SerializeField] ParticleSystem muzzleFlash;
	[SerializeField] GameObject enemy;

	[Header("AI Vars")]
	public bool shooting = false;
	float lastFiredTime = 0f;

	void Start() {
		recoil = GetComponent<RecoilController>();
		stats = GetComponent<GunStats>();
	}

	void Update() {
		if (shooting) {
			Shoot();
		}
		UpdateShootState();
	}

	void UpdateShootState() {
		DiscoverPlayer dp = enemy.GetComponent<DiscoverPlayer>();
		if (Physics.Raycast(raycaster.transform.position, raycaster.transform.forward, dp.viewDist, dp.targetMask)) {
			shooting = true;
		}
		else shooting = false;
	}

	void FixedUpdate() {
		recoil.UpdateRecoil();
	}

	void Shoot() {
		if (Time.time >= lastFiredTime) {
			recoil.fire = true;
			muzzleFlash.Play();
			raycaster.ShootBullet(stats, muzzleFlash.transform);
			lastFiredTime = Time.time + stats.fireRate;
		}
	}
}
