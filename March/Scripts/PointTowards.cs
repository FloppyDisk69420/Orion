using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTowards : MonoBehaviour {
	public Transform thing;
	
	void Update() {
		// if thing exists
		if (thing) {
			Vector3 lookPos = thing.position- transform.position;
			transform.rotation = Quaternion.LookRotation(lookPos, Vector3.up);
		}
	}
}
