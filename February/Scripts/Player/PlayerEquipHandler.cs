using System.Collections;
using UnityEngine;

public class PlayerEquipHandler : MonoBehaviour {
	InputManager input;
	public LayerMask pickupLayer;
	public Transform fpsCam;
	public WeaponHolder weapons;

	[Header("Settings")]
	public float pickupRadius = 6f;
	public float pickupAngle = 45f;
	public float threshold = 10f;

	[Header("OnPickup")]
	public float pickupBase = 10f;
	public float pickupMag = 3f;
	public float minMag = 0.1f;

	void Start() {
		input = InputController.Instance.input;

		input.Player.Interact.performed += _ => PickupWeapon();
	}

	void PickupWeapon() {
		// if holding a weapon we cant pickup another weapon
		switch (weapons.weaponType) {
			case WeaponHolder.WeaponType.Primary:
				if (weapons.primaryWeapon.props) {
					Debug.Log("Can't pickup weapon because you are already holding one");
					return;
				}
			break;
			case WeaponHolder.WeaponType.Secondary:
				if (weapons.secondaryWeapon.props) {
					Debug.Log("Can't pickup weapon because you are already holding one");
					return;
				}
			break;
			case WeaponHolder.WeaponType.Melee:
				if (weapons.meleeWeapon.stats) {
					Debug.Log("Can't pickup weapon because you are already holding one");
					return;
				}
			break;
		}
		
		Collider[] pickups = Physics.OverlapSphere(transform.position, pickupRadius, pickupLayer);

		// decide on the best pickup to pickup

		// each pickup is given points to determine the best pickup to pickup
		float[] points = new float[pickups.Length];
		for (int i = 0; i < pickups.Length; i++) {
			Collider pickup = pickups[i];
			
			// get distance between the pickup and the player
			float dist = Vector3.Distance(transform.position, pickup.transform.position);
			// makes it so that the closer the pickup is the bigger the value is
			points[i] = pickupRadius - dist;
			
			// find the angle between the player look and the pickup
			Vector3 relativePos = pickup.transform.position - fpsCam.position;
			float angle = Vector3.Angle(fpsCam.forward, relativePos);
			// if angle is small the points awarded is larger
			points[i] += pickupAngle - angle;
		}		

		int bestIndex = 0;
		float bestPoint = float.NegativeInfinity;
		for (int i = 0; i < points.Length; i++) {
			if (points[i] > bestPoint) {
				bestPoint = points[i];
				bestIndex = i;
			}
		}

		if (bestPoint >= threshold) {
			Debug.Log("We picked up " + pickups[bestIndex].name);
			StartCoroutine(DoPickup(pickups[bestIndex].transform));
		}
		else {
			Debug.Log("Could not pickup anything");
		}
	}

	IEnumerator DoPickup(Transform pickup) {
		// cancel pickup if melee (not supported yet)
		if (weapons.weaponType == WeaponHolder.WeaponType.Melee) yield break;
		
		// disable rotation because we will set it to zero
		pickup.GetComponent<PickupController>().rotate = false;

		// get weapon type transform
		Transform currWeapon = weapons.transform.Find(weapons.weaponType.ToString());
		
		float dist = float.PositiveInfinity;
		float angle = float.PositiveInfinity;
		while (dist > 0.001f && angle > 0.001f) {
			// move to player
			dist = Vector3.Distance(pickup.position, currWeapon.position);
			float m = dist.Map(0, pickupRadius, minMag, pickupMag);
			float pSpeed = Time.deltaTime * pickupBase * m;
			pickup.position = Vector3.Lerp(pickup.position, currWeapon.position, pSpeed);
			// rotate to normal
			angle = Quaternion.Angle(pickup.rotation, Quaternion.Euler(0, 0, 0));
			float aSpeed = Time.deltaTime * pickupBase * pickupBase;
			pickup.rotation = Quaternion.Slerp(pickup.rotation, Quaternion.Euler(0, 0, 0), aSpeed);
			// wait for next frame
			yield return null;
		}
		// set the position and rotation to be exact
		pickup.position = currWeapon.position;
		pickup.rotation = Quaternion.Euler(0, 0, 0);
		// the pickup contents is always located at zero
		Transform contents = pickup.GetChild(0);
		// set the parent to the right weapon type
		contents.SetParent(currWeapon);
		currWeapon.GetComponent<GunController>().props = contents.GetComponent<GunProperties>();
		weapons.UpdateWeapons();
		// destroy pickup leftovers
		pickup.GetComponent<PickupController>().Delete();
	}

	void ThrowWeapon() {
		// TODO make a throwable object class to throw the weapon and do damage
		// TODO When touch the ground make it a pickup
		Debug.Log("Throwing weapon...");
	}
}
