using UnityEngine;
using System.Collections;

public class Raycaster : MonoBehaviour {
	[Header("Serialize")]
	[SerializeField] Transform fpsCam;
	[SerializeField] GameObject bulletPrefab;
	[SerializeField] LayerMask targetMask;

	// Shoots a raycast out to the center of camera
	public void ShootBullet(GunStats stats, Transform t) {
		Collider col = bulletPrefab.GetComponent<Collider>();
		// get simplified version of radius
		float radius = (col.bounds.size.x + col.bounds.size.y) / 2;
		// we shoot a raycast which get the point of contact 
		RaycastHit hit;
		Vector3 pointOfContact;
		if (Physics.SphereCast(fpsCam.position, radius, fpsCam.forward, out hit, stats.range, targetMask)) {
			pointOfContact = hit.point;
			Debug.Log("Raycast hit something!");
		}
		else {
			pointOfContact = fpsCam.position + fpsCam.forward * stats.range;
			Debug.Log("Raycast did not hit something!");
		}

		// we rotate the bullet to face the point of contact
		GameObject bullet = Instantiate(bulletPrefab, t.position, LookTo(pointOfContact, t.position));
		BulletController bc = bullet.GetComponent<BulletController>(); 
		bc.targetMask = targetMask;
		bc.stats = stats;
	}

	public Quaternion LookTo(Vector3 point, Vector3 position) {
		Vector3 relativePos = point - position;
        return Quaternion.LookRotation(relativePos, Vector3.up);
	}
}