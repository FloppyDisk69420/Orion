using UnityEngine;

public class ExplosiveImpact : BulletImpact {
	public GameObject explosion;
	public float explosionRadius = 15f;
	public float explosionPower = 12f;
	public float lethality = 10f;
	public float lift = 0.4f;

	public override void OnImpact(RaycastHit hit) {
		Instantiate(explosion, hit.point, Quaternion.Euler(0, 0, 0));
		
		// get objects in radius
		Collider[] colliders = Physics.OverlapSphere(hit.point, explosionRadius, bc.targetMask);

		foreach (Collider collider in colliders) {
			// apply force if theres a rigidbody 
			Rigidbody rb;
			if (collider.TryGetComponent<Rigidbody>(out rb))
				rb.AddExplosionForce(explosionPower, hit.point, explosionRadius, lift, ForceMode.Impulse);
			
			// do damage if damageable
			EntityStats eStats;
			if (collider.TryGetComponent<EntityStats>(out eStats)) {
				float dist = Vector3.Distance(hit.point, collider.transform.position);
				// multiplier that has larger values the closer it is to the explosion center
				float damageMult = (explosionRadius - dist).Map(0, explosionRadius, 0, lethality);
				float damageDone = explosionPower * lethality;
				eStats.TakeDamage(damageDone);
			}
		}
	}
}
