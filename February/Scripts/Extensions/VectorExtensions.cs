using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axes {
	X,
	Y,
	Z
}

public static class Vector3Extensions {
	// Simple rotation around an axis
	public static Vector3 Rotate(this Vector3 v, Axes axes, float degree) {
		float cos = Mathf.Cos(degree * Mathf.Deg2Rad);
		float sin = Mathf.Sin(degree * Mathf.Deg2Rad);
		
		Vector3 newV = new Vector3();
		switch (axes) {
			case Axes.X:
				newV.x = v.x;
				newV.y = v.y * cos - v.z * sin;
				newV.z = v.y * sin + v.z * cos;
			break;
			case Axes.Y:
				newV.x = v.x * cos + v.z * sin;
				newV.y = v.y;
				newV.z = -v.x * sin + v.z * cos;
			break;
			case Axes.Z:
				newV.x = v.x * cos - v.y * sin;
				newV.y = v.x * sin + v.y * cos;
				newV.z = v.z;
			break;
		}
		return newV;
	}
}

public static class Vector2Extensions {
}