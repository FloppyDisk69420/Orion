using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {
	public List<Transform> interests = new List<Transform>();
	public List<Vector3> points = new List<Vector3>();
	public Vector3 average = new Vector3();

	public bool samePriority = false;
	public float priority = 1f;

	void UpdateInterests() {
		points.Clear();
		average = Vector3.zero;

		foreach (Transform interest in interests) {
			Vector3 point = interest.position - transform.position;

			if (samePriority) {
				point = Vector3.ClampMagnitude(point, priority);
			}

			points.Add(point);
			average += point;
		}
		average /= interests.Count;
	}

	void Start() {
		UpdateInterests();
	}

	void Update() {
		transform.localRotation = Quaternion.LookRotation(average, Vector3.up);

		for (int i = 0; i < interests.Count; i++) {
			if (interests[i].position - transform.position != points[i]) {
				UpdateInterests();
				break;
			}
		}
	}
}
