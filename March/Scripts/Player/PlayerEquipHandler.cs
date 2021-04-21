using System.Collections;
using UnityEngine;

public class PlayerEquipHandler : MonoBehaviour {
	InputManager input;
	public Transform fpsCam;
	public WeaponHolder weapons;

	[Header("Pickup Settings")]
	public float pickupRadius = 6f;
	public float pickupAngle = 45f;
	public float threshold = 10f;

	[Header("On Pickup Settings")]
	public float pickupSpeed = 1.5f;
	public float rotationSpeed = 1.5f;

	[Header("Weapon Pickup References")]
	public ComplexRecoil camRecoil;
	public Raycaster raycaster;

	void Start() {
		input = InputController.Instance.input;

		input.Player.Interact.performed += _ => DetermineWhichPickup();
		input.Player.Interact.canceled += _ => ReleaseObject();
		// when throw key is released, the weapon is thrown
		input.Player.Throw.canceled += _ => ThrowWeapon();

		newestObject = null;
	}

	// chooses whether the player wishes to pickup a pickup or an object
	void DetermineWhichPickup() {
		if (weapons.weaponType == WeaponHolder.WeaponType.Melee)
			PickupObject();
		else
			PickupWeapon();
	}

	ThrowableObject newestObject = null;
	void ReleaseObject() {
		if (newestObject) {
			newestObject.isPickedUp = false;
			newestObject = null;
		}
	}

	void PickupObject() {
		if (weapons.weaponType != WeaponHolder.WeaponType.Melee) {
			Debug.Log("You can only pickup objects with your melee weapon!");
			return;
		}
		
		LayerMask mask = LayerMask.GetMask("Projectile");
		Collider[] objects = Physics.OverlapSphere(transform.position, pickupRadius, mask);
		
		// decide on the best object to pickup

		// each object is given points to determine the best object to pickup
		float[] points = new float[objects.Length];
		for (int i = 0; i < objects.Length; i++) {
			Collider obj = objects[i];
			if (obj.GetComponentInParent<ThrowableObject>().pickupable) {
				// get distance between the object and the player
				float dist = Vector3.Distance(transform.position, obj.transform.position);
				// makes it so that the closer the object is the bigger the value is
				points[i] = pickupRadius - dist;
				
				// find the angle between the player look and the object
				Vector3 relativePos = obj.transform.position - fpsCam.position;
				float angle = Vector3.Angle(fpsCam.forward, relativePos);
				// if angle is small the points awarded is larger
				points[i] += pickupAngle - angle;
			}
			else {
				points[i] = float.NegativeInfinity;
			}
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
			Debug.Log("We picked up " + objects[bestIndex].name);
			objects[bestIndex].GetComponent<ThrowableObject>().isPickedUp = true;
		}
		else {
			Debug.Log("Could not pickup any object");
		}
	}

	void PickupWeapon() {
		// if holding a weapon we cant pickup another weapon
		if (HoldingWeapon()) {
			Debug.Log("Can't pickup weapon because you are already holding one");
			return;
		}
		GunController curController = weapons.GetGun();
		if (curController.reserved) {
			Debug.Log("Slot " + curController.name + "is reserved");
			return;
		}
		
		LayerMask mask = LayerMask.GetMask("Pickup");
		Collider[] pickups = Physics.OverlapSphere(transform.position, pickupRadius, mask);

		// decide on the best pickup to pickup

		// each pickup is given points to determine the best pickup to pickup
		float[] points = new float[pickups.Length];
		for (int i = 0; i < pickups.Length; i++) {
			Collider pickup = pickups[i];
			if (pickup.GetComponentInParent<PickupController>().type == PickupController.PickupType.Wieldable) {
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
			else {
				points[i] = float.NegativeInfinity;
			}
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
			Debug.Log("Could not pickup any pickup");
		}
	}

	IEnumerator DoPickup(Transform pickup) {
		// cancel pickup if melee (not supported yet)
		if (weapons.weaponType == WeaponHolder.WeaponType.Melee) yield break;
		
		// find the pickup controller which is supposed to be at the base of the object
		PickupController pc = pickup.GetComponentInParent<PickupController>();
		
		// disable rotation because we will set it to zero
		pc.rotate = false;

		// the particle effect is supposed to be the second child of the pickup
		Transform effect = pc.transform.GetChild(1);
		effect.GetComponent<ParticleSystem>().Pause();
		effect.GetComponent<ParticleSystem>().Stop();

		// get weapon type transform
		Transform currWeapon = weapons.GetGun().transform;
		// reserve the weapon slot
		currWeapon.GetComponent<GunController>().reserved = true;

		float rotTime = 0f;
		Quaternion startRot = pc.transform.rotation;
		float moveTime = 0f;
		Vector3 startPos = pc.transform.position;

		while (rotTime != 1f && moveTime != 1f) {
			// update time by elapsed time and speed
			rotTime += Time.deltaTime * rotationSpeed;
			moveTime += Time.deltaTime * pickupSpeed;
			// clamp the times between 0 and 1
			rotTime = Mathf.Clamp01(rotTime);
			moveTime = Mathf.Clamp01(moveTime);
			// update positions
			pc.transform.position = Vector3.Lerp(startPos, currWeapon.position, moveTime);
			pc.transform.rotation = Quaternion.Slerp(startRot, fpsCam.rotation, rotTime);

			yield return null;
		}
		// set values to be exact
		pc.transform.position = currWeapon.position;
		pc.transform.rotation = fpsCam.rotation;
		
		// get the weapon
		GameObject weapon = pc.transform.GetChild(0).gameObject;
		weapon.SetActive(true);
		// set the parent to the right weapon type
		weapon.transform.SetParent(currWeapon);
		// set all children and self to weapon layer
		weapon.SetAllToLayer(LayerMask.NameToLayer("Weapon"));

		// Update and set properties
		ShootGun sg = weapon.transform.GetComponent<ShootGun>();
		sg.camRecoil = camRecoil;
		sg.raycaster = raycaster;

		GunController con = currWeapon.GetComponent<GunController>();
		con.UpdateWeapon();
		weapon.GetComponent<GunProperties>().SetWeapon(true);
		weapons.UpdateWeapons();
		
		// destroy pickup leftovers
		pc.Delete();
	}

	bool HoldingWeapon() {
		// checks if the properties of the weapon exists
		switch (weapons.weaponType) {
			case WeaponHolder.WeaponType.Primary:
				if (weapons.primaryWeapon.props)
					return true;
			break;
			case WeaponHolder.WeaponType.Secondary:
				if (weapons.secondaryWeapon.props)
					return true;
			break;
			case WeaponHolder.WeaponType.Melee:
				if (weapons.meleeWeapon.stats)
					return true;
			break;
		}
		return false;
	}

	[Header("Throwable Weapon Settings")]
	public LayerMask throwMask;
	public float throwForce = 15f;
	public GameObject pickupEffect;
	void ThrowWeapon() {
		if (weapons.weaponType == WeaponHolder.WeaponType.Melee) {
			Debug.Log("Can't throw weapon because melee is not supported yet");
			return;
		}
		
		if (!HoldingWeapon()) {
			Debug.Log("Can't throw weapon because you are not holding one.");
			return;
		}

		// get weapon type transform
		Transform currWeapon = weapons.GetGun().transform;
		// get weapon
		Transform weapon = currWeapon.GetChild(0);
		
		// create a throwable object
		GameObject twObject = new GameObject(weapon.name);
		twObject.transform.position = weapon.position;
		twObject.transform.rotation = weapon.rotation;

		ThrowableObject to = twObject.AddComponent<ThrowableObject>();
		// when collided make object a pickup
		to.whenCollided = (hit) => {
			// first calculate damage done
			EntityStats eStats;
			if (hit.collider.TryGetComponent<EntityStats>(out eStats)) {
				eStats.TakeDamage(to.rb.velocity.magnitude);
			}
			
			CreatePickup(to);
		};
		to.rb = twObject.AddComponent<Rigidbody>();
		to.cam = fpsCam.transform;
		to.targetMask = throwMask;
		// objects in motion stay in motion
		to.rb.velocity = GetComponent<Movement>().rb.velocity;
		// increase velocity in direction player is looking at
		to.rb.AddForce(fpsCam.forward * throwForce, ForceMode.Impulse);
		to.rb.AddTorque(Random.insideUnitSphere * throwForce, ForceMode.Impulse);

		// we take the weapon from the player and make it part of the throwable
		weapon.SetParent(twObject.transform, true);
		weapon.localPosition = Vector3.zero;
		weapon.localRotation = Quaternion.identity;
		weapon.gameObject.SetActive(true);

		// update weapon info and disable weapon
		GunController con = currWeapon.GetComponent<GunController>();		
		con.UpdateWeapon();
		weapon.GetComponent<GunProperties>().SetWeapon(true);
		weapons.UpdateWeapons();

		// set all children and self to Projectile layer
		twObject.SetAllToLayer(LayerMask.NameToLayer("Projectile"));
	}

	void CreatePickup(ThrowableObject to) {
		// create a pickup
		GameObject pickup = new GameObject(to.name + " Pickup");
		// move to just above ground
		pickup.transform.position = to.rb.position + Vector3.up * 0.1f;
		pickup.transform.rotation = to.transform.rotation;

		// makes it so that it is equipable
		PickupController pc = pickup.AddComponent<PickupController>();
		pc.type = PickupController.PickupType.Wieldable;
		pc.targetMask = throwMask;
		pc.rotate = true;

		// transfer the weapon contents to the pickup
		Transform tWeapon = to.transform.GetChild(0);
		tWeapon.SetParent(pickup.transform);
		tWeapon.localPosition = Vector3.zero;
		tWeapon.localRotation = Quaternion.identity;
		tWeapon.gameObject.SetActive(true);

		// create pickup effect
		GameObject effects = Instantiate(pickupEffect);
		effects.name = pickupEffect.name;
		effects.GetComponent<ParticleSystem>().Play();
		effects.transform.SetParent(pickup.transform);
		effects.transform.localRotation = Quaternion.identity;
		BoxCollider bc = tWeapon.GetComponentInChildren<BoxCollider>();
		effects.transform.localPosition = bc.center;

		// set all children and pickup to pickup layer
		pickup.SetAllToLayer(LayerMask.NameToLayer("Pickup"));

		// destroy the throwable game object
		Destroy(to.gameObject);
	}
}
