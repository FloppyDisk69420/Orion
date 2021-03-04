using UnityEngine;

[RequireComponent(typeof(HealthManager))]
public class EntityStats : MonoBehaviour {
	[Header("Acceleration")]
	public float walkSpeed = 2000f;
	public float sprintSpeed = 4000f;
	public float slideSpeed = 400f;

	[Header("Limits")]
	public float maxClimbAngle = 35f;
	public float maxWalkSpeed = 10f;
	public float maxSprintSpeed = 20f;
	[Space()]
	public float jumpCooldown = 0.25f;
	public float maxHealth = 100f;

	[Header("Current")]
	public float health = 100f;
	public float agility = 1.0f;
	public float jumpForce = 500f;

	void Start() {
		health = maxHealth;
	}

	public void TakeDamage(float damage) {
		health -= damage;
		health = Mathf.Clamp(health, 0, maxHealth);
		GetComponent<HealthManager>().SendMessage("UpdateHealth");
	}

	public void Heal(float healAmount) {
		health += healAmount;
		health = Mathf.Clamp(health, 0, maxHealth);
		GetComponent<HealthManager>().SendMessage("UpdateHealth");
	}
}