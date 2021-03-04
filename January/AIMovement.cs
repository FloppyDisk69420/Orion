using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : Movement {
	bool suspicuous, alerted;
	bool followRoute;

	// Route variables
	public Route route;
	int i;

	new void Start() {
		base.Start();
		suspicuous = false;
		alerted = false;
		followRoute = true;
		i = 0;
	}

	new void FixedUpdate() {
		if (followRoute && route.checkpoints != null) {
			if (route.checkpoints.Count > 0) {
				DoRoute();
			}
		}
		base.FixedUpdate();
	}

	void DoRoute() {
		if (HitCheckpoint()) {
			if (i < route.checkpoints.Count - 1) {
				i++;
			}
			else {
				i = 0;
			}
		}
		LookToCheckpoint();
		move.y = 1;
	}

	bool HitCheckpoint() {
		Vector2 topDownEnemy = new Vector2(transform.position.x, transform.position.z);
		Vector2 topDownRoute = new Vector2(route.checkpoints[i].x, route.checkpoints[i].z);
		float dist = Vector2.Distance(topDownEnemy, topDownRoute);
		return (dist < 0.5f);
	}

	void LookToCheckpoint() {
		Vector3 relativePos = route.checkpoints[i] - transform.position;
		transform.rotation = Quaternion.LookRotation(relativePos, Vector3.up);
		transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
	}
}
