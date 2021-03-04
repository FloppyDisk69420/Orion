using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
	public float smoothTime = .2f;
	
	Animator animator;
	PlayerMovement player;
	
    void Start() {
        player = GetComponent<PlayerMovement>();
		animator = GetComponentInChildren<Animator>();
    }
    void Update() {
		if (player.sprinting && player.speed.magnitude > 0) animator.SetFloat("SpeedNormal", 1f, smoothTime, Time.deltaTime);
		else if (player.speed.magnitude > 0) animator.SetFloat("SpeedNormal", 0.5f, smoothTime, Time.deltaTime);
		else animator.SetFloat("SpeedNormal", 0f, smoothTime, Time.deltaTime);
    }
}
