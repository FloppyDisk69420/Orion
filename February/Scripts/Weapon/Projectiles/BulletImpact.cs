using UnityEngine;

public class BulletImpact : MonoBehaviour {
	[SerializeField] protected BulletController bc;
	
	public virtual void OnImpact(RaycastHit hit) {
		Debug.Log("You shot " + hit.collider.name + "!");
		EntityStats eStats;
		if (hit.collider.TryGetComponent<EntityStats>(out eStats)) {
			eStats.TakeDamage(bc.stats.bulletDamage);
		}
	}
}
