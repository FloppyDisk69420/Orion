using UnityEngine;
using System.Collections;

public class Raycaster : MonoBehaviour {
	public Camera fpsCam;
	public GameObject bulletPrefab;
	
	// Shoots a raycast out to the center of camera
	public void ShootBullet(GunStats stats, Transform transform) {
		GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
		bullet.GetComponent<BulletController>().stats = stats;
		RaycastHit hit;
		// if the raycast hits something...
		if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, stats.range)) {
			PointTowards(bullet, hit.point);
			bullet.GetComponent<BulletController>().isReady = true;
		}
		else {
			PointTowards(bullet, fpsCam.transform.forward * stats.range + fpsCam.transform.position);
			bullet.GetComponent<BulletController>().isReady = true;
		}
	}

	void PointTowards(GameObject obj, Vector3 point) {
		Vector3 relativePos = point - obj.transform.position;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        obj.transform.rotation = rotation;
	}
}