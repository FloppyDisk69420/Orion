using UnityEngine;
using UnityEngine.UI;

public class ADSController : MonoBehaviour {
	InputManager input;
	[Header("Static")]
	public Camera fpsCam;
	public GameObject weaponCam;
	public GameObject crosshair;
	public Image reticle; 
	[HideInInspector]
	public GunProperties gunProps;

	bool fullyScoped = false;

	void Start() {
		input = InputController.Instance.input;
		fullyScoped = false;

		input.Player.ADS.performed += _ => StartScope();
		input.Player.ADS.canceled += _ => StopScope();
	}

	void Update() {
		DoCamZoom();
		ScopeCheck();
	}

	void ScopeCheck() {
		if (gunProps) {
			if (gunProps.animator.IsDone(0, "Aim")) {
				if (!fullyScoped) {
					fullyScoped = true;
					OnScoped();
				}
			}
			else {
				fullyScoped = false;
			}
		}
	}

	public void OnDefault() {
		if (gunProps) {
			weaponCam.SetActive(true);
			if (gunProps.stats.aimType == ADSType.Scope) {
				crosshair.SetActive(false);
			}
			else {
				crosshair.SetActive(true);
			}

			reticle.gameObject.SetActive(false);
		}
	}

	void OnScoped() {
		if (gunProps) {
			if (gunProps.stats.aimType == ADSType.Scope) {
				// if weapon has scope, show it
				if (gunProps.stats.aimType == ADSType.Scope) {
					reticle.sprite = gunProps.scope;
					reticle.gameObject.SetActive(true);
				}
				// make gun invisible
				weaponCam.SetActive(false);
			}
			crosshair.SetActive(false);
		}
	}

	// handle zooming in/out
	void DoCamZoom() {
		if (gunProps) {
			// if aiming down sight
			if (gunProps.animator.GetBool("ADS")) {
				// zoom in
				float maxOffset = gunProps.stats.hipFOV - gunProps.stats.adsFOV;
				float offset = fpsCam.fieldOfView - gunProps.stats.adsFOV;
				float zoomMult = offset.Map(0, maxOffset, 0, gunProps.stats.scopeInSpeed);

				fpsCam.fieldOfView -= gunProps.stats.aimStep * zoomMult;
			}
			else {
				// zoom out
				float maxOffset = gunProps.stats.hipFOV - gunProps.stats.adsFOV;
				float offset = gunProps.stats.hipFOV - fpsCam.fieldOfView;
				float zoomMult = offset.Map(0, maxOffset, 0, gunProps.stats.scopeInSpeed);

				fpsCam.fieldOfView += gunProps.stats.aimStep * zoomMult;
			}
		}
	}

	void StartScope() {
		if (gunProps) {
			gunProps.animator.SetBool("ADS", true);
		}
	}

	void StopScope() {
		if (gunProps) {
			gunProps.animator.SetBool("ADS", false);
			if (gunProps.stats.aimType == ADSType.Scope) {
				weaponCam.SetActive(true);
			}
			else if (gunProps.stats.aimType == ADSType.Aim) {
				crosshair.SetActive(true);
			}
			reticle.gameObject.SetActive(false);
		}
	}
}
