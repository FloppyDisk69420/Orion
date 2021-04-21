using UnityEngine;

public class BulletImpact : MonoBehaviour {
	public BulletController bc;
	
	public virtual void OnImpact(RaycastHit hit) {
		EntityStats eStats;
		if (hit.collider.TryGetComponent<EntityStats>(out eStats)) {
			eStats.TakeDamage(bc.stats.bulletDamage);
		}
	}
}
