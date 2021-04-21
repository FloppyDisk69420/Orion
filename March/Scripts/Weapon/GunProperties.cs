using UnityEngine;

// just some things the guns need for pickups
public class GunProperties : MonoBehaviour {
	public Sprite scope;
	public GameObject bullet;
	public ShootGun controller;
	public GunStats stats;
	public Animator animator;

	public void SetWeapon(bool isActive) {
		animator.enabled = isActive;
		controller.enabled = isActive;
	}
}
