using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour {
	[Header("Base Settings")]
	public string title = "Ability";
	public float cooldown = 1f;
	public bool unlocked = false;
	public bool ableToUse = false;

	void Start() {
		title = "Ability";
		held = false;
		ableToUse = false;
		OnStart();
	}

	void Update() {
		if (unlocked) {
			OnUpdate();
			if (held) {
				OnHold();
			}
		}
	}

	public string GetActionName() {
		// removes all spaces in ability name
		return title.Replace(" ", null);
	}

	public void AbleToUse() {
		ableToUse = true;
	}

	// called when scene is started
	public virtual void OnStart() {}

	// called when keybind is pressed
	public virtual void OnPressed() {}

	// called when bind is released
	public virtual void OnReleased() {}
	
	// colled every frame
	public virtual void OnUpdate() {}

	// colled every frame when keybinf is held
	public virtual void OnHold() {}

	// used to determine whether keybind is held down
	public bool held = false;
}
