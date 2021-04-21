using UnityEngine;

public static class GameObjectExtensions {
	public static bool IsPrefab(this GameObject obj) {
		return obj.scene.rootCount == 0;
	}

	public static void SetAllToLayer(this GameObject obj, LayerMask layer) {
		Transform[] ts = obj.GetComponentsInChildren<Transform>();

		foreach (Transform t in ts) {
			t.gameObject.layer = layer;
		}
	}
}