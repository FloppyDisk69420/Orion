using UnityEngine;

public class EntityStats : MonoBehaviour {
	// "acceleration" vars
	public float walkSpeed = 2000f;
	public float sprintSpeed = 4000f;
	public float slideSpeed = 400f;

	public float jumpCooldown = 0.25f;
	public float maxClimbAngle = 35f;

	public float maxWalkSpeed = 10f;
	public float maxSprintSpeed = 20f;

	public float agility = 1.0f;
	public float health = 100f;
	public float jumpForce = 500f;

}
