using System.Collections.Generic;
using UnityEngine;

public class Route: MonoBehaviour {
	// assignables
	[HideInInspector()]
	public List<Vector3> checkpoints = new List<Vector3>();
	public List<float> times = new List<float>();
	public void AddCheckpoint() {
		checkpoints.Add(new Vector3());
		times.Add(0.1f);
		checkpoints[checkpoints.Count - 1] = transform.position + Vector3.forward;
	}

	public void RemoveCheckpoint(int index) {
		if (index >= 0 && index < checkpoints.Count) {
			checkpoints.RemoveAt(index);
			times.RemoveAt(index);
		}
	}
}