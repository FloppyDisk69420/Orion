using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SlowMotion : Ability {
	[Header("Slow Motion Settings")]
	public Volume volume;
	public float maxVignetteStrength = 0.3f;
	
	public float slowFactor = 20f;
	bool slowed = false;

	Vignette vignette;
	public override void OnStart() {
		title = "Slow Motion";
		slowed = false;
		defaultFixedDeltaTime = 0.02f;

		volume.profile.TryGet(out vignette);
		if (vignette) {
			vignette.intensity.value = 0f;
		}
	}

	float defaultFixedDeltaTime = 0.02f;
	public override void OnPressed() {
		base.OnPressed();
		slowed = !slowed;
	}

	public float time = 0f;
	public float inMult = 2f;
	public float outMult = 2f;
	public override void OnUpdate() {
		float normalTime = 1f;
		float slowTime = normalTime / slowFactor;
		time = Mathf.Clamp01(time);

		// update scales
		Time.timeScale = normalTime + time * (slowTime - normalTime);
		Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;

		// update post-processing effects
		if (vignette) {
			vignette.intensity.value = time * maxVignetteStrength;

			if (vignette.intensity.value == 0)
				vignette.active = false;
			else
				vignette.active = true;
		}

		// update time
		if (slowed)
			time += Time.unscaledDeltaTime * inMult;
		else
			time -= Time.unscaledDeltaTime * outMult;
		
		// make sure time is between 0 and 1
		time = Mathf.Clamp01(time);
	}
}
