using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ADSType {
	Scope,
	Aim,
}

public enum BulletType {
	Small,
	Shotgun,
	Medium,
	Large,
	Elemental,
}

public class GunStats : MonoBehaviour {
	[Header("Base")]
	public float bulletDamage = 150f;
	public float range = 100f;

	[Header("Shoot Speed")]
	public float fireRate = 0.15f;
	public bool auto = false;

	[Header("Reload Settings")]
	public BulletType type;
	public int magazine = 1;
	public int shotCost = 1;
	public int ammo = 1000;
	public float reloadTime = 1f;

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
