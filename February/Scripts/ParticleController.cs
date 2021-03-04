using UnityEngine;

public class ParticleController : MonoBehaviour {
	ParticleSystem particle;

	void Awake() {
		particle = GetComponent<ParticleSystem>();
	}

	void Update() {
		if (!particle.IsAlive()) {
			particle.Stop();
			Destroy(gameObject);
		}
	}
}
