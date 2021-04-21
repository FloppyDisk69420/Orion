using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {
	public EntityStats stats;
	public HealthBar healthBar;

	void Start() {
		healthBar.SetMaxHealth(stats.maxHealth);
		healthBar.SetHealth(stats.health);
	}

	public void UpdateHealth() {
		healthBar.SetHealth(stats.health);
		Debug.Log("Entity: " + stats.name + " HP:" + stats.health);
	}
}
