using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : WeaponController {
	[SerializeField] LayerMask attackableMask;
	public MeleeStats stats;
	Collider other;
	
	void Start() {
		// weapons ignore ground, obstacle, player, and enemy layer
		Physics.IgnoreLayerCollision(12, 8);
		Physics.IgnoreLayerCollision(12, 9);
		Physics.IgnoreLayerCollision(12, 10);
		Physics.IgnoreLayerCollision(12, 11);
	}

	void OnTriggerEnter(Collider c) {
		if (attackableMask == (attackableMask | (1 << c.gameObject.layer)))
			other = c;
		else
			other = null;
	}

	void OnTriggerExit() {
		other = null;
	}
	
	public override void Attack() {
		if (gameObject.activeSelf) {
			// if there was a collision to an attackable object...
			if (other) {
				EntityStats s;
				// if damageable
				if (other.gameObject.TryGetComponent<EntityStats>(out s)) {
					s.TakeDamage(stats.damage);
				}
			}
		}
	}
}
