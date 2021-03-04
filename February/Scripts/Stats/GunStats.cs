using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ADSType {
	Scope,
	Aim,
}

public class GunStats : MonoBehaviour {
	[Header("Base")]
	public float bulletDamage = 150f;
	public float range = 100f;

	[Header("Shoot Speed")]
	public float fireRate = 0.15f;
	public bool auto = false;

	[Header("Multipliers")]
	public float recoil = 1f;
	public float bulletSpeed = 3f;

	[Header("Camera")]
	public ADSType aimType = ADSType.Aim;
	public float hipFOV = 60f;
	public float adsFOV = 40f;
	public float scopeInSpeed = 4f;
	public float scopeOutSpeed = 12f;
	public float aimStep = 0.1f;
}
