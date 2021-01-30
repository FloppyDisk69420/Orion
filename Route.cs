using System.Collections.Generic;
using UnityEngine;

public class Route: MonoBehaviour {
	// assignables
	[HideInInspector()]
	public List<Vector3> checkpoints;

	void Start() {
		checkpoints = new List<Vector3>();
	}

	public void AddCheckpoint() {
		checkpoints.Add(new Vector3());
	}

	public void RemoveCheckpoint(int index) {
		if (index >= 0 && index < checkpoints.Count) {
			checkpoints.RemoveAt(index);
		}
	}
}