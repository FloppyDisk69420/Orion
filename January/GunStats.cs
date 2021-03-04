using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunStats : MonoBehaviour {

	public float bulletDamage = 150f;
	public float range = 100f;
	public Transform heldPoint;
	public float fireRate = 0.15f;
	public bool auto = false;

	// multipliers
	public float recoil = 1f;
	public float bulletSpeed = 1000f;
}
