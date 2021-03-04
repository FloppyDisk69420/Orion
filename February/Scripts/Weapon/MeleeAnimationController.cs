using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAnimationController : MonoBehaviour {
	InputManager input;
	[SerializeField] Animator animator;

	void Start() {
		input = InputController.Instance.input;

		input.Player.Fire.performed += _ => DoSlashAnim();
		input.Player.Fire.canceled += _ => DoIdleAnim();
	}

	void DoIdleAnim() {
		animator.SetInteger("SlashState", 0);
	}

	void DoSlashAnim() {
		animator.SetInteger("SlashState", 1);
	}
}
