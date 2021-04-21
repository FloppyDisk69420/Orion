using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Dash : Ability {
	public Transform fpsCam;
	public Rigidbody playerRb;
	public float dashStrength = 10f;
	
	public override void OnStart() {
		title = "Dash";
	}

	public override void OnPressed() {
		playerRb.AddForce(fpsCam.forward * dashStrength, ForceMode.Impulse);
	}
}
