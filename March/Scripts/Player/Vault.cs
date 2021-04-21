using UnityEngine;
using System.Collections;

public class Vault : MonoBehaviour {
	[Header("Assign")]
	public Rigidbody rb;
	public EntityStats stats;
	public Transform feetPos;

	[Header("Settings")]
	public LayerMask vaultableMask;
	public float checkDist = 1f;
	public float vaultDist = 1f;
	public float vaultForce = 100f;
	public float vaultSpeed = 2f;
	public float vaultMaxSpeed = 2f;
	public float maxDist = 10f;

	Collider colliderToVault;

	void Awake() {
		rb = GetComponent<Rigidbody>();
		stats = GetComponent<EntityStats>();
	}

	void Update() {
		CheckIfVaultable();
	}

	void CheckIfVaultable() {
		CapsuleCollider collider = GetComponent<CapsuleCollider>();
		
		RaycastHit hit;
		if (Physics.Raycast(feetPos.position, rb.transform.forward, out hit, checkDist, vaultableMask)) {
			float top = hit.collider.bounds.max.y;
			float dist = top - feetPos.position.y;
			Debug.Log(dist);
			// if distance from top is less than the vault distance
			if (dist <= vaultDist && dist > collider.height / 2) {
				colliderToVault = hit.collider;
				DoVault();
			}
		}
	}

	void DoVault() {
		
	}
}
