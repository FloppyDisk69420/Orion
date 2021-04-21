using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateWeaponPickup : MonoBehaviour {
	public GameObject weapon;
	public GameObject pickupEffect;
	public LayerMask mask;

	public void CreatePickup() {
			name = weapon.name + " Pickup";
			// makes it so that it is equipable
			PickupController pc = gameObject.AddComponent<PickupController>();
			pc.type = PickupController.PickupType.Wieldable;
			pc.targetMask = mask;
			pc.rotate = true;

			// transfer the weapon contents to the pickup
			Transform tWeapon = Instantiate(weapon).transform;
			tWeapon.SetParent(transform);
			tWeapon.localPosition = Vector3.zero;
			tWeapon.localRotation = Quaternion.identity;
			tWeapon.gameObject.SetActive(true);
			tWeapon.name = weapon.name;
			// disable weapon scripts
			tWeapon.GetComponent<GunProperties>().SetWeapon(false);


			// create pickup effect
			if (pickupEffect) {
				GameObject effects = Instantiate(pickupEffect);
				effects.name = pickupEffect.name;
				effects.GetComponent<ParticleSystem>().Play();
				effects.transform.SetParent(transform);
				effects.transform.localRotation = Quaternion.identity;
				BoxCollider bc = tWeapon.GetComponentInChildren<BoxCollider>();
				effects.transform.localPosition = bc.center;

			}

			// set all children and pickup to pickup layer
			gameObject.SetAllToLayer(LayerMask.NameToLayer("Pickup"));

			// delete this component (since this should be done in edit mode, we use DestoryImmediate)
			DestroyImmediate(GetComponent<CreateWeaponPickup>());
	}
}
