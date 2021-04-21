using UnityEngine;

// A container that is used to spawn weapons.
public class WeaponContainer : MonoBehaviour {
	public GameObject weapon; 

	// spawns a weapon then delete the container
	public GameObject CreateWeapon() {
		// create weapon
		GameObject newWeapon = Instantiate(weapon, transform.position, transform.rotation);
		newWeapon.name = weapon.name;

		if (!weapon.IsPrefab())
			Destroy(weapon);
		// destroy the container
		Destroy(gameObject);
		
		return newWeapon;
	}
}
