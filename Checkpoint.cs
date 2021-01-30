using UnityEngine;
using System.Collections.Generic;

public struct Checkpoint {
	public Vector3 position;

	public Checkpoint(Vector3 pos) {
		position = pos;
	}

	public void Move(float x, float y, float z) {
		position += new Vector3(x, y, z);
	}

	public void Move(Vector3 move) {
		position += move;
	}
}