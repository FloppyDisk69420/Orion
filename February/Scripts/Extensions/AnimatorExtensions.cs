using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorExtensions {
	public static bool IsDone(this Animator animator, int layer, string animation) {
		bool isDone = animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= .99f;
		bool notInTransition = !animator.IsInTransition(layer);
		bool rightAnimation = animator.GetCurrentAnimatorStateInfo(layer).IsName(animation);

		return isDone && notInTransition && rightAnimation;
	}
}
