using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilityManager : MonoBehaviour {
	InputManager input;
	Ability[] abilities;
	
	public GameObject abilityManager;

	void Start() {
		input = InputController.Instance.input;
		abilities = abilityManager.GetComponents<Ability>();
		SetAbilities();
	}

	void OnAbilityPressed(Ability ability) {
		if (ability.unlocked && ability.ableToUse) {
			ability.held = true;
			ability.OnPressed();
			ability.ableToUse = false;
		}
	}

	void OnAbilityReleased(Ability ability) {
		if (ability.unlocked && ability.held) {
			ability.held = false;
			ability.OnReleased();
			ability.Invoke("AbleToUse", ability.cooldown);
		}
	}

	void SetAbilities() {
		foreach (Ability ability in abilities) {
			// gets the action
			InputAction action = input.Skills.Get().FindAction(ability.GetActionName());
			action.performed += _ => OnAbilityPressed(ability);
			action.canceled += _ => OnAbilityReleased(ability);
			
			if (ability.unlocked)
				// make able to use in cooldown seconds
				ability.Invoke("AbleToUse", ability.cooldown);
		}
	}
}
