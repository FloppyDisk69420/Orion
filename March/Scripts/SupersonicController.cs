using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupersonicController : MonoBehaviour {
	public ParticleSystem effects;
	public Rigidbody rb;


	public float minVelocity = 15f;

	void Update() {
		if (rb.velocity.magnitude >= minVelocity) {
			if (!effects.isPlaying) {
				effects.Play();
			}
		}
		else {
			if (effects.isPlaying) {
				effects.Stop();
			}
		}
	}
}
